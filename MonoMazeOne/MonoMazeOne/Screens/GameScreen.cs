using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NorthGame.Core.Abstractions;
using NorthGame.Core.Game;
using NorthGame.Tiled;

namespace MonoMazeOne.Screens
{
    public class GameScreen : GameScreenBase
    {
        private readonly IGameElementFactory _factory;
        private readonly IInputManager _inputManager;
        public string ViewLayout { get; set; } = "Design\\Viewscreen";
        public string MazeLayout { get; set; } = "MazeLevels\\Zero";
        public Direction MoveDirection { get; private set; }

        private readonly TiledMap _view = new TiledMap();
        private readonly TiledMap _maze = new TiledMap();

        public GameScreen(IGameElementFactory factory, IInputManager inputManager)
        {
            _factory = factory;
            _inputManager = inputManager;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            _view.LoadContent(ViewLayout, _factory, false);
            _maze.LoadContent(MazeLayout, _factory, true);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            _view.UnloadContent();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            _maze.Draw(spriteBatch, new Vector2(32, 32), new Vector2(176, 176));
            _view.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_inputManager.KeyDown(Keys.Down))
            {
                MoveDirection = Direction.Down;
            }
            else if (_inputManager.KeyDown(Keys.Up))
            {
                MoveDirection = Direction.Up;
            }
            else if (_inputManager.KeyDown(Keys.Left))
            {
                MoveDirection = Direction.Left;
            }
            else if (_inputManager.KeyDown(Keys.Right))
            {
                MoveDirection = Direction.Right;
            }
            else
            {
                MoveDirection = Direction.None;
            }

            // map updates after player updates
            // to handle map changes due to movement.
            _maze.Update(gameTime, MoveDirection);
            _view.Update(gameTime);

            Status = $"{loopCount++}";
        }

        private int loopCount = 0;
    }
}
