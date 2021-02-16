using Microsoft.Xna.Framework;
using NorthGame.Core.Abstractions;
using NorthGame.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoMazeOne
{
    public class GameRules : IGameRules
    {
        private readonly IScoreManager _scoreManager;
        
        public GameRules(IScoreManager scoreManager)
        {
            _scoreManager = scoreManager;
        }

        private IMapLayer _mapLayer;
        private Point _layerSize;
        public void RestartMap(IMapLayer mapLayer)
        {
            _mapLayer = mapLayer;
            _layerSize = new Point(mapLayer.Width, mapLayer.Height);
            _scoreManager.Reset();
            _scoreManager.Lost = _layerSize.CountMatrix((column, row) => mapLayer.Tiles[row * _layerSize.X + column].TileState == TileState.Diamond);
            _scoreManager.Found = 0;
        }

        public void ApplyGameRules(Direction playerMoveDirection)
        {
            // Reset activity
            _layerSize.VisitMatrix((column, row) => _mapLayer.Tiles[row * _layerSize.X + column].Active = true);

            // traverse from bottom to top, to prevent falling objects from falling throw in one run.
            ListExtensions.VisitMatrix(_layerSize, (column, row) =>
            {
                var tile = _mapLayer.Tiles[row * _layerSize.X + column];
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
                    _mapLayer.PlayerPosition = new Point(column, row);
                    ITile nextTile = null;
                    switch (playerMoveDirection)
                    {
                        case Direction.Up:
                            tile.TileState = TileState.PlayerUp;
                            nextTile = _mapLayer.Tiles[(row - 1) * _layerSize.X + column];
                            break;

                        case Direction.Left:
                            tile.TileState = TileState.PlayerLeft;
                            nextTile = _mapLayer.Tiles[row * _layerSize.X + column - 1];
                            break;

                        case Direction.Right:
                            tile.TileState = TileState.PlayerRight;
                            nextTile = _mapLayer.Tiles[row * _layerSize.X + column + 1];
                            break;

                        case Direction.Down:
                            tile.TileState = TileState.PlayerDown;
                            nextTile = _mapLayer.Tiles[(row + 1) * _layerSize.X + column];
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
                            _scoreManager.Lost--;
                            _scoreManager.Found++;
                            nextTile.TileState = TileState.None;
                            SwapTiles(tile, nextTile);
                        }
                        else if (nextTile.TileState == TileState.Rock)
                        {
                            ITile moveRock = null;
                            if (playerMoveDirection == Direction.Left)
                            {
                                moveRock = _mapLayer.Tiles[row * _layerSize.X + column - 2];
                            }
                            else if (playerMoveDirection == Direction.Right)
                            {
                                moveRock = _mapLayer.Tiles[row * _layerSize.X + column + 2];
                            }
                            if (moveRock != null && moveRock.TileState == TileState.None)
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
            var tileBelow = _mapLayer.Tiles[(row + 1) * _layerSize.X + column];
            if (tileBelow.TileState == TileState.None)
            {
                SwapTiles(tile, tileBelow);
            }
            // roll off rocks and diamonds
            else if (tileBelow.TileState == TileState.Rock || tileBelow.TileState == TileState.Diamond)
            {
                var tileRight = _mapLayer.Tiles[row * _layerSize.X + column - 1];
                var tileRightBelow = _mapLayer.Tiles[(row + 1) * _layerSize.X + column - 1];
                if (tileRight.TileState == TileState.None && tileRightBelow.TileState == TileState.None)
                {
                    SwapTiles(tile, tileRight);
                }
                else
                {
                    var tileLeft = _mapLayer.Tiles[row * _layerSize.X + column + 1];
                    var tileLeftBelow = _mapLayer.Tiles[(row + 1) * _layerSize.X + column + 1];
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
    }
}
