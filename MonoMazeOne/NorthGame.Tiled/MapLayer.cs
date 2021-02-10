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


        public void Draw(SpriteBatch spriteBatch, Vector2 offset, Vector2 screenoffset, Vector2 viewSize)
        {
            var maxX = offset.X + viewSize.X;
            var maxY = offset.Y + viewSize.Y;
            var cellSize = 16;

            Tiles
                .Where(m => m.Active)
                .Visit(m =>
                {
                    if (TileSheet.Position.X < offset.X || TileSheet.Position.Y < offset.Y)
                    {
                        // hidden
                    }
                    else
                    {
                        // draw at an offset
                        TileSheet.Position = m.Position + screenoffset;
                        if (TileSheet.Position.X > maxX || TileSheet.Position.Y > maxY)
                        {
                            // hidden
                        }
                        else
                        {
                            int spriteIndex = (int)m.TileState-1;
                            if (spriteIndex > 0)
                            {
                                TileSheet.SourceRect = new Rectangle((spriteIndex % 8) * cellSize, (spriteIndex / 8) * cellSize, 16, 16);
                                TileSheet.Draw(spriteBatch);
                            }
                        }
                    }
                });
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

        public void LoadContent(Point tileSize, IGameElementFactory tileFactory, ITileSet tileSet, bool colisionMap)
        {
            TileDimension = tileSize;
            _layerSize = new Point(Width, Height);
            _tileOffset = new Vector2(TileDimension.X / 2, TileDimension.Y / 2);

            // TODO: set hitbox size for different tile types.
            _tileHitbox = new Rectangle(-TileDimension.X / 2, -TileDimension.Y / 2, TileDimension.X, TileDimension.Y);

            _tileSet = tileSet;

            var shortName = Path.GetFileNameWithoutExtension(tileSet.Source);
            TileSheet = tileFactory.CreateSprite($"TileSets/{shortName}");
            TileSheet.LoadContent();

            LoadMap(tileFactory, colisionMap);
        }

        public void UnloadContent()
        {
            Tiles.Clear();
            TileSheet.UnloadContent();
        }

        const int speed = 250;
        TimeSpan speedTimer; 
        public void Update(GameTime gameTime)
        {
            // handle tile changes
            speedTimer += gameTime.ElapsedGameTime;
            if (speedTimer.TotalMilliseconds > speed)
            {
                UpdateTileChanges();
                speedTimer = 0;
            }
        }

        private void UpdateTileChanges()
        {
            // traverse from bottom to top, to prevent falling objects from falling throw in one run.
            ListExtensions.VisitMatrixReverseY(_layerSize, (column, row) => { 
                var tile = Tiles[row * _layerSize.X + column];
                if (tile.TileState==TileState.Rock)
                {
                    // rocks fall and roll of each other
                    var tileBelow = Tiles[(row+1) * _layerSize.X + column];
                    if (tileBelow.TileState == TileState.None)
                    {
                        // fall down
                        tileBelow.TileState = TileState.Rock;
                        tile.TileState = TileState.None;
                    }
                }
            });
        }

        private void LoadMap(IGameElementFactory tileFactory, bool collisionMap)
        {
            // Interpret zplane
            if ( Enum.TryParse(Name, out LayerType z))
            {
                ZPlane = z;
            }

            if (Type == "tilelayer")
            {
                // Read tiles
                Tiles.Clear();
                var k = 0;
                var l = 0;
                foreach (var s in Data)
                {
                    var tile = tileFactory.CreateTile();
                    ParseTileInformation(ref tile, new Point(l, k), s, collisionMap, 8);
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

        private ITile ParseTileInformation(
            ref ITile tile, 
            Point mapPosition, 
            int spriteSheetIndex, 
            bool collisionMap,
            int tilesetColumns)
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
                        state = TileState.None;
                    }
                    tile.TileState = state;
                }

                // Correction for image offset               
                spriteSheetIndex -= 1;
                var  sx = (spriteSheetIndex % tilesetColumns) * TileDimension.X;                
                var sy = (spriteSheetIndex / tilesetColumns) * TileDimension.Y;

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
