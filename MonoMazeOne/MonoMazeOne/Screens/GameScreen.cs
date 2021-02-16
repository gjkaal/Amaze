using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NorthGame.Core.Abstractions;
using NorthGame.Core.Game;
using NorthGame.Tiled;
using System;

namespace MonoMazeOne.Screens
{
    public class GameScreen : GameScreenBase
    {
        private readonly IGameElementFactory _factory;
        private readonly IInputManager _inputManager;
        private readonly IScoreManager _scoreManager;
        private readonly IGameRules _gameRules;
        public string ViewLayout { get; set; } = "Design\\Viewscreen";
        public string MazeLayout { get; set; } = "MazeLevels\\Zero";
        private Direction moveDirection;
        private Direction nextMove;
        private double speedTimer;
        private bool firstRunPassed;

        private readonly TiledMap _view = new TiledMap();
        private readonly TiledMap _maze = new TiledMap();
        private ISprite _lost;
        private ISprite _found;

        public GameScreen(
            IGameElementFactory factory,
            IInputManager inputManager,
            IScoreManager scoreManager,
            IGameRules gameRules)
        {
            _factory = factory;
            _inputManager = inputManager;
            _gameRules = gameRules;
            _scoreManager = scoreManager;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            _view.LoadContent(ViewLayout, _factory);
            _maze.LoadContent(MazeLayout, _factory);
            _lost = _factory.CreateSprite(string.Empty);
            _found = _factory.CreateSprite(string.Empty);

            _lost.Text = "lost";
            _lost.Position = new Vector2(244, 96);
            _lost.LoadContent();

            _found.Text = "found";
            _found.Position = new Vector2(244, 144);
            _found.LoadContent();

            _gameRules.RestartMap(_maze.Layers[0]);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            _view.UnloadContent();
            _lost.UnloadContent();
            _found.UnloadContent();
            _maze.UnloadContent();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            _maze.Draw(spriteBatch, new Vector2(32, 32), new Vector2(176, 176));
            _view.Draw(spriteBatch);
            _lost.Draw(spriteBatch);
            _found.Draw(spriteBatch);
        }

        private int _lostValue;
        private int _foundValue;
        public override void Update(GameTime gameTime)
        {
            if (_lostValue != _scoreManager.Lost)
            {
                _lost.UnloadContent();
                _lost= _factory.CreateSprite(string.Empty);
                _lost.Position = new Vector2(244, 96);
                _lost.Text = _scoreManager.Lost.ToString();
                _lost.LoadContent();
                _lostValue = _scoreManager.Lost;
            }

            if (_foundValue != _scoreManager.Found)
            {
                _found.UnloadContent();
                _found = _factory.CreateSprite(string.Empty);
                _found.Position = new Vector2(244, 144);
                _found.Text = _scoreManager.Found.ToString();
                _found.LoadContent();
                _foundValue = _scoreManager.Found;
            }


            base.Update(gameTime);

            if (_inputManager.KeyDown(Keys.Down))
            {
                moveDirection = Direction.Down;
            }
            else if (_inputManager.KeyDown(Keys.Up))
            {
                moveDirection = Direction.Up;
            }
            else if (_inputManager.KeyDown(Keys.Left))
            {
                moveDirection = Direction.Left;
            }
            else if (_inputManager.KeyDown(Keys.Right))
            {
                moveDirection = Direction.Right;
            }
            else
            {
                moveDirection = Direction.None;
            }

            // map updates after player updates
            // to handle map changes due to movement.
            _maze.Update(gameTime);
            _view.Update(gameTime);            

            Status = $"{loopCount++}";

            // handle tile changes
            if (moveDirection != Direction.None)
            {
                nextMove = moveDirection;
            }

            var timeSlice = Math.Round(gameTime.TotalGameTime.TotalSeconds * 3, 0);
            if (timeSlice > speedTimer && firstRunPassed)
            {
                _gameRules.ApplyGameRules(nextMove);
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

        private int loopCount = 0;
    }
}
