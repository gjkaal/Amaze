// Rapbit Game development
//
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NorthGame.Core.Abstractions;
using NorthGame.Core.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NorthGame.Tiled
{
    public class MapLayer : IGameElement, IMapLayer
    {
        // An objectgroup contains objects, no data
        // A tile layer contains data, no objects
        public List<VectorObject> Objects = new List<VectorObject>();

        public List<ITile> Tiles { get; } = new List<ITile>();

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

        public Point PlayerPosition { get; set; } = Point.Zero;

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

        public void Draw(SpriteBatch spriteBatch, Vector2 screenoffset, Vector2 viewSize)
        {
            Vector2 offset = Vector2.Zero;
            if (PlayerPosition.X >= 4)
            {
                offset.X = (PlayerPosition.X - 4) * 16;
            }
            if (PlayerPosition.Y >= 4)
            {
                offset.Y = (PlayerPosition.Y - 4) * 16;
            }
            var x = Width - PlayerPosition.X;
            if (x <= 5)
            {
                offset.X = (Width - 9) * 16;
            }
            var y = Height - PlayerPosition.Y;
            if (y <= 5)
            {
                offset.Y = (Height - 9) * 16;
            }
            offset -= screenoffset;

            Tiles
                .Where(m => m.TileState != TileState.None)
                .Visit(m =>
                {
                    // draw at an offset
                    TileSheet.Position = m.Position - offset;
                    var maxX = TileSheet.Position.X + viewSize.X;
                    var maxY = TileSheet.Position.Y + viewSize.Y;

                    if (TileSheet.Position.X > maxX || TileSheet.Position.Y > maxY)
                    {
                        // hidden
                    }
                    else
                    {
                        DrawSprite(spriteBatch, TileSheet, m);
                    }
                });
        }

        private static void DrawSprite(SpriteBatch spriteBatch, ISprite tileSheet, ITile m)
        {
            const int cellSize = 16;
            int spriteIndex = (int)m.TileState - 1;
            if (spriteIndex > 0)
            {
                tileSheet.SourceRect = new Rectangle((spriteIndex % 8) * cellSize, (spriteIndex / 8) * cellSize, cellSize, cellSize);
                tileSheet.Draw(spriteBatch);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Tiles
                .Where(m => m.TileState != TileState.None)
                .Visit(m =>
                {
                    TileSheet.Position = m.Position;
                    DrawSprite(spriteBatch, TileSheet, m);
                });
        }

        public void LoadContent()
        {
            throw new NotSupportedException();
        }

        public void LoadContent(Point tileSize, IGameElementFactory tileFactory, ITileSet tileSet)
        {
            TileDimension = tileSize;
            _tileOffset = new Vector2(TileDimension.X / 2, TileDimension.Y / 2);

            // TODO: set hitbox size for different tile types.
            _tileHitbox = new Rectangle(-TileDimension.X / 2, -TileDimension.Y / 2, TileDimension.X, TileDimension.Y);

            _tileSet = tileSet;

            var shortName = Path.GetFileNameWithoutExtension(tileSet.Source);
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
            // empty
        }

        private void LoadMap(IGameElementFactory tileFactory)
        {
            // Interpret zplane
            if (Enum.TryParse(Name, out LayerType z))
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
                    ParseTileInformation(ref tile, new Point(l, k), s);
                    Tiles.Add(tile);
                    l++;
                    if (l >= Width)
                    {
                        k += 1;
                        l = 0;
                    }
                }
            }
            if (Type == "objectgroup")
            {
                // TODO: read objects for EXIT and START locations
            }
        }

        private ITile ParseTileInformation(
            ref ITile tile,
            Point mapPosition,
            int spriteSheetIndex)
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
                tile.TileState = TileState.None;
            }
            else
            {
                if (spriteSheetIndex <= (int)TileState.MaxTileState)
                {
                    tile.TileState = (TileState)spriteSheetIndex;
                }
                else
                {
                    tile.TileState = TileState.None;
                }
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
