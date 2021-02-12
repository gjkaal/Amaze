// Rapbit Game development
//
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NorthGame.Core;
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
        private readonly IScoreManager _scoreManager;

        public MapLayer()
        {
            _scoreManager = new ScoreManager();
        }

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

        public void Draw(SpriteBatch spriteBatch, Vector2 screenoffset, Vector2 viewSize)
        {
            Vector2 offset = Vector2.Zero;
            if (playerPosition.X >= 4)
            {
                offset.X = (playerPosition.X - 4) * 16;
            }
            if (playerPosition.Y >= 4)
            {
                offset.Y = (playerPosition.Y - 4) * 16;
            }
            var x = _layerSize.X - playerPosition.X;
            if (x<=5)
            {
                offset.X = (_layerSize.X - 9) * 16;
            }
            var y = _layerSize.Y - playerPosition.Y;
            if (y <= 5)
            {
                offset.Y = (_layerSize.Y - 9) * 16;
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

        private bool firstRunPassed;
        private double speedTimer;
        private Direction nextMove;
        private Point playerPosition;

        public void Update(GameTime gameTime, Direction playerMoveDirection)
        {
            // handle tile changes
            if (playerMoveDirection != Direction.None)
            {
                nextMove = playerMoveDirection;
            }

            var timeSlice = Math.Round(gameTime.TotalGameTime.TotalSeconds * 3, 0);
            if (timeSlice > speedTimer && firstRunPassed)
            {
                UpdateTileChanges(nextMove);
                // set timer for next run
                speedTimer = timeSlice;
                // reset movement
                nextMove = Direction.None;
            }
            else
            {
                firstRunPassed = true;
            }
        }

        public void Update(GameTime gameTime)
        {
            // empty
        }

        private void UpdateTileChanges(Direction playerMoveDirection)
        {
            // Reset activity
            _layerSize.VisitMatrix((column, row) => Tiles[row * _layerSize.X + column].Active = true);

            // traverse from bottom to top, to prevent falling objects from falling throw in one run.
            ListExtensions.VisitMatrix(_layerSize, (column, row) =>
            {
                var tile = Tiles[row * _layerSize.X + column];
                // skip if tile already moved
                if (!tile.Active) return;
                if (tile.TileState == TileState.Rock)
                {
                    HandleRockMovement(column, row, tile);
                }
                if (tile.TileState == TileState.PlayerDown
                || tile.TileState == TileState.PlayerUp
                || tile.TileState == TileState.PlayerLeft
                || tile.TileState == TileState.PlayerRight)
                {
                    playerPosition = new Point(column, row);
                    ITile nextTile = null;
                    switch (playerMoveDirection)
                    {
                        case Direction.Up:
                            tile.TileState = TileState.PlayerUp;
                            nextTile = Tiles[(row - 1) * _layerSize.X + column];
                            break;

                        case Direction.Left:
                            tile.TileState = TileState.PlayerLeft;
                            nextTile = Tiles[row * _layerSize.X + column - 1];
                            break;

                        case Direction.Right:
                            tile.TileState = TileState.PlayerRight;
                            nextTile = Tiles[row * _layerSize.X + column + 1];
                            break;

                        case Direction.Down:
                            tile.TileState = TileState.PlayerDown;
                            nextTile = Tiles[(row + 1) * _layerSize.X + column];
                            break;
                    }
                    // check if moving and move possible
                    if (nextTile != null)
                    {
                        if (nextTile.TileState == TileState.None)
                        {
                            SwapTiles(tile, nextTile);
                        }
                        else if (nextTile.TileState == TileState.Dirt)
                        {
                            nextTile.TileState = TileState.None;
                            SwapTiles(tile, nextTile);
                        }
                        else if (nextTile.TileState == TileState.Diamond)
                        {
                            _scoreManager.AddScore(1, 1);
                            nextTile.TileState = TileState.None;
                            SwapTiles(tile, nextTile);
                        }
                        else if (nextTile.TileState == TileState.Rock)
                        {
                            ITile moveRock=null;
                            if (playerMoveDirection == Direction.Left)
                            {
                                moveRock = Tiles[row * _layerSize.X + column -2];
                            }else if (playerMoveDirection == Direction.Right)
                            {
                                moveRock = Tiles[row * _layerSize.X + column + 2];
                            }
                            if (moveRock!=null && moveRock.TileState == TileState.None)
                            {
                                // move rock and player
                                SwapTiles(nextTile, moveRock);
                                SwapTiles(tile, nextTile);
                            }
                        }
                    }
                }
            });
        }

        private void HandleRockMovement(int column, int row, ITile tile)
        {
            // rocks fall and roll of each other
            var tileBelow = Tiles[(row + 1) * _layerSize.X + column];
            if (tileBelow.TileState == TileState.None)
            {
                SwapTiles(tile, tileBelow);
            }
            // roll off rocks and diamonds
            else if (tileBelow.TileState == TileState.Rock || tileBelow.TileState == TileState.Diamond)
            {
                var tileRight = Tiles[row * _layerSize.X + column - 1];
                var tileRightBelow = Tiles[(row + 1) * _layerSize.X + column - 1];
                if (tileRight.TileState == TileState.None && tileRightBelow.TileState == TileState.None)
                {
                    SwapTiles(tile, tileRight);
                }
                else
                {
                    var tileLeft = Tiles[row * _layerSize.X + column + 1];
                    var tileLeftBelow = Tiles[(row + 1) * _layerSize.X + column + 1];
                    if (tileLeft.TileState == TileState.None && tileLeftBelow.TileState == TileState.None)
                    {
                        SwapTiles(tile, tileLeft);
                    }
                }
            }
        }

        private static void SwapTiles(ITile tile, ITile other)
        {
            var state = tile.TileState;
            tile.TileState = other.TileState;
            other.TileState = state;
            // prevent movement on the same run
            other.Active = false;
        }

        private void LoadMap(IGameElementFactory tileFactory, bool collisionMap)
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
                if (spriteSheetIndex < (int)TileState.MaxTileState)
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
