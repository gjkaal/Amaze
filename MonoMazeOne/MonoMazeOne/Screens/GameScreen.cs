using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NorthGame.Core.Abstractions;
using NorthGame.Core.Game;
using NorthGame.Tiled;

namespace MonoMazeOne.Screens
{
    public class GameScreen : GameScreenBase
    {
        private readonly IGameElementFactory _factory;
        public string ViewLayout { get; set; } = "Design\\Viewscreen";
        public string MazeLayout { get; set; } = "MazeLevels\\Zero";

        private readonly TiledMap _view = new TiledMap();
        private readonly TiledMap _maze = new TiledMap();
        public GameScreen(IGameElementFactory factory)
        {
            _factory = factory;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            _view.LoadContent(ViewLayout, _factory);
            _maze.LoadContent(MazeLayout, _factory);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            _view.UnloadContent();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            _maze.Draw(spriteBatch, new Vector2 (0, 0), new Vector2(32, 32), new Vector2(176, 176));
            _view.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            // map updates after player updates
            // to handle map changes due to movement.
            _maze.Update(gameTime);
            _view.Update(gameTime);

            Status = $"{loopCount++}";
        }

        private int loopCount = 0;
    }
}
