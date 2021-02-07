using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NorthGame.Core;
using NorthGame.Core.Abstractions;
using NorthGame.Core.ContainerService;
using System;

namespace MonoMazeOne
{
    public class GameOne : Game
    {
        public const int ViewWidth= 9;
        public const int ViewHeight= 9;
        public const int ViewOffsetH = 3;
        public const int ViewOffsetV = 3;
        public const int TileSize = 16;
        public const int ScreenWidth = 320;
        public const int ScreenHeigth = 240;

        public const string RootNamespace = "MonoMazeOne";
        public const string RootNamespaceDot = RootNamespace + ".";
        public const string ScreensNamespace = RootNamespaceDot + "Screens";

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private readonly IScreenManager _screen;

        public GameOne()
        {
            SetupContainer();
            _screen = NorthGameContainer.Instance.Resolve<IScreenManager>();
            _graphics = NorthGameContainer.Instance.Resolve<GraphicsDeviceManager>();
            IsMouseVisible = true;

            IsMouseVisible = true;

            _screen.ScreenResolver = ((screen) => {
                var screenType = Type.GetType($"{ ScreensNamespace}.{screen}");

                if (screenType == null) throw new NotImplementedException($"Screen type not found : {ScreensNamespace}.{screen}");
                if (!typeof(IGameScreen).IsAssignableFrom(screenType)) throw new NotImplementedException($"Type is not valid as game screen");

                return screenType;

            });
        }

        private void SetupContainer()
        {
            Content.RootDirectory = Constants.DesignRoot;
            NorthGameContainer.Instance.SetUp((c) =>
            {
                // Game runtime items
                c.Register(() => new ContentManager(Content.ServiceProvider, Constants.DesignRoot), SimpleInjector.Lifestyle.Singleton);
                c.Register(() => new GraphicsDeviceManager(this), SimpleInjector.Lifestyle.Singleton);

                // GameElementFactory registration here 
                // gives the possibility to modify default behavior.
                c.Register<IGameElementFactory, GameElementFactory>(SimpleInjector.Lifestyle.Singleton);
                // Game design elements
                c.Register<Screens.GameScreen>();
            }, () =>
            {
                return new NorthGameConfiguration
                {
                    ScreenWidth = 320.0f,
                    ScreenHeight = 240.0f
                };
            });
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = (int)_screen.Dimensions.X;
            _graphics.PreferredBackBufferHeight = (int)_screen.Dimensions.Y;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _screen.Graphics = GraphicsDevice;
            _screen.Sprites = _spriteBatch;
            _screen.CurrentScreen = NorthGameContainer.Instance.Resolve<Screens.GameScreen>();
            _screen.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            { 
                SaveAndExit(); 
            }

            _screen.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            _screen.Draw(_spriteBatch);
            _spriteBatch.End();
            base.Draw(gameTime);
        }



        protected override void UnloadContent()
        {
            _screen.UnloadContent();
            base.UnloadContent();
        }

        private void SaveAndExit()
        {
            UnloadContent();
            Exit();
        }
    }
}
