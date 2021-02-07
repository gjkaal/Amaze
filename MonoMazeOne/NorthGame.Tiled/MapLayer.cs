// Rapbit Game development
//
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using NorthGame.Core.Extensions;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using NorthGame.Core.Abstractions;

namespace NorthGame.Tiled
{
    public class MapLayer : IGameElement, IMapLayer
    {
        // An objectgroup contains objects, no data
        // A tile layer contains data, no objects
        public List<VectorObject> Objects = new List<VectorObject>();

        public readonly List<ITile> Tiles = new List<ITile>();

        private Point _layerSize;

        private ITileSet _tileSet;

        private Rectangle _tileHitbox;

        private Vector2 _tileOffset;

        // The Data contains the references to the spritemap for all tile coordinates
        public int[] Data { get; set; }

        // Height and Width are set for tile layers.
        // The value is the number of tiles
        public int Height { get; set; }

        public int Id { get; set; }

        public string Layout { get => Name; set => Name = value; }

        //-------------------
        // Model parameters
        public string Name { get; set; }

        public float Opacity { get; set; }

        public Point TileDimension { get; private set; }

        //-------------------
        public ISprite TileSheet { get; set; }

        /// <summary>
        /// Layer type, either [tilelayer] or [objectgroup]
        /// </summary>
        public string Type { get; set; }

        public int Width { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public LayerType ZPlane { get; private set; }

        public bool CheckTileCollision(IPlayer player, out ITile tile, out Rectangle hitBox)
        {
            // get tiles near position of the player.
            tile = null;
            hitBox = Rectangle.Empty;
            Vector2 playerVelocity = player.Velocity;
            var playerRef = player.GridReference();

            if (playerVelocity.Equals(Vector2.Zero)) return false;


            for (var k = -player.TileSpan.Y; k <= player.TileSpan.Y; k++)
            {
                var row = playerRef.Y + k;
                // skip rows outside of tile map.
                if (row < 0 || row >= _layerSize.Y) continue;
                for (var l = -player.TileSpan.X; l <= player.TileSpan.X; l++)
                {
                    var column = playerRef.X + l;
                    if (column < 0 || column >= _layerSize.X) continue;
                    // find the tile
                    var matchTile = Tiles[row * _layerSize.X + column];
                    if (!matchTile.Active || matchTile.TileState == TileState.None) continue;

                    // check minkowski collision
                    var playerVector = player.PlayerPosition() + player.Velocity;
                    var colliderSize = Core.Math.NorthMath.Minkowski(
                        column * TileDimension.X,
                        row * TileDimension.Y,
                        _tileOffset,
                        player.HitBox,
                        _tileHitbox);

                    var checkVector = new Vector2((int)playerVector.X, (int)playerVector.Y);
                    if (colliderSize.Contains(checkVector))
                    {
                        hitBox = _tileHitbox;
                        tile = matchTile;
                        return true;
                    }
                }
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Tiles
                .Where(m => m.Active)
                .Visit(m =>
                {
                    TileSheet.Position = m.Position;
                    TileSheet.SourceRect = m.SourceRect;
                    TileSheet.Draw(spriteBatch);
                });
        }

        public void LoadContent()
        {
            throw new NotSupportedException();
        }

        public void LoadContent(Point tileSize, IGameElementFactory tileFactory, ITileSet tileSet)
        {
            TileDimension = tileSize;
            _layerSize = new Point(Width, Height);
            _tileOffset = new Vector2(TileDimension.X / 2, TileDimension.Y / 2);

            // TODO: set hitbox size for different tile types.
            _tileHitbox = new Rectangle(-TileDimension.X / 2, -TileDimension.Y / 2, TileDimension.X, TileDimension.Y);

            _tileSet = tileSet;

            var shortName = Path.GetFileNameWithoutExtension(tileSet.Name);
            TileSheet = tileFactory.CreateSprite($"TileSets/{shortName}");
            TileSheet.LoadContent();

            LoadMap(tileFactory);
        }

        public void UnloadContent()
        {
            Tiles.Clear();
            TileSheet.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
        }

        private void LoadMap(IGameElementFactory tileFactory)
        {
            // Interpret zplane
            if ( Enum.TryParse(Name, out LayerType z))
            {
                ZPlane = z;
            }

            if (Type == "tilelayer")
            {
                // check layer type
                var collisionLayer = Name.Equals("COLLISION", StringComparison.OrdinalIgnoreCase);
                // Read tiles
                Tiles.Clear();
                var k = 0;
                var l = 0;
                foreach (var s in Data)
                {
                    var tile = tileFactory.CreateTile();
                    ParseTileInformation(ref tile, new Point(l, k), s, collisionLayer);
                    Tiles.Add(tile);
                    l++;
                    if (l >= Width)
                    {
                        k += 1;
                        l = 0;
                    }                    
                }
            }
            if (Type== "objectgroup")
            {
                // TODO: read objects for EXIT and START locations
            }
        }

        private ITile ParseTileInformation(ref ITile tile, Point mapPosition, int spriteSheetIndex, bool collisionMap)
        {
            var x = mapPosition.X;
            var y = mapPosition.Y;
            var tx = x * TileDimension.X;
            var ty = y * TileDimension.Y;

            tile.Position = new Vector2((float)tx, (float)ty);
            // Set the position for the tile and
            // get tile location within the tilesheet
            if (spriteSheetIndex == 0)
            {                
                tile.Active = false;
                tile.TileState = TileState.None;
            }
            else
            {
                if (collisionMap)
                {
                    // Set tilestate to something else then collide
                    TileState state;
                    if (spriteSheetIndex < (int)TileState.MaxTileState)
                    {
                        state = (TileState)spriteSheetIndex;
                    }
                    else
                    {
                        state = TileState.Solid;
                    }
                    tile.TileState = state;
                }
                // Correction for image offset
                spriteSheetIndex -= 1;
                var sx = (spriteSheetIndex % _tileSet.Columns) * _tileSet.TileWidth;
                var sy = (spriteSheetIndex / _tileSet.Columns) * _tileSet.TileHeight;

                tile.Active = true;

                tile.SourceRect = new Rectangle(sx, sy, TileDimension.X, TileDimension.Y);                
            }
            return tile;
        }

        private struct State
        {
            public TileState TileState { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }
    }
}
