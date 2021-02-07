using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NorthGame.Core.Abstractions;
using NorthGame.Core.Game;
using NorthGame.Tiled;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoMazeOne.Screens
{
    public class GameScreen : GameScreenBase
    {
        private readonly IGameElementFactory _factory;
        public string ViewLayout { get; set; } = "Design\\Viewscreen";
        private readonly TiledMap _view = new TiledMap();
        public GameScreen(IGameElementFactory factory)
        {
            _factory = factory;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            _view.LoadContent(ViewLayout, _factory);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            _view.UnloadContent();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            _view.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            // map updates after player updates
            // to handle map changes due to movement.
            _view.Update(gameTime);
        }
    }
}
