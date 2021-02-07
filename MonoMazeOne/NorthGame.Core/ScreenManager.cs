// Rapbit Game development
//
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NorthGame.Core.Abstractions;
using NorthGame.Core.ContainerService;
using NorthGame.Core.Extensions;
using NorthGame.Core.Game;
using System;

namespace NorthGame.Core
{

    public sealed class ScreenManager : IScreenManager
    {
        public IGameScreen CurrentScreen { get; set; }
        private IGameScreen _nextScreen;
        public Vector2 Dimensions { get; set; } = new Vector2(Constants.ScreenWidth, Constants.ScreenHeight);
        public ISprite FadeImage { get; set; }
        public ISprite Fader => FadeImage;

        public GraphicsDevice Graphics { get; set; }
        /// <summary>
        /// If true, then the screen is transiotioning to another screen
        /// </summary>
        public bool IsTransitioning { get; private set; }

        public string Layout { get; set; } // = "Design\\ScreenManager";
        public Type ScreenType { get; private set; }
        public SpriteBatch Sprites { get; set; }
        public ScreenManager(INorthGameConfiguration config)
        {
            Dimensions = new Vector2(config.ScreenWidth, config.ScreenHeight);            
        }


        public Func<string, Type> ScreenResolver { private get; set; }

        public Vector2 CenterOffset(Vector2 size)
        {
            return new Vector2((Dimensions.X - size.X) / 2, (Dimensions.Y - size.Y) / 2);
        }

        public void ChangeScreen(string screen)
        {
            var nextScreenType = ScreenResolver(screen);

            _nextScreen = NorthGameContainer.Instance.ResolveScreen(nextScreenType);

            FadeImage.LoadContent();
            FadeImage.IsActive = true;
            FadeImage.FadeEffect.Increase = true;
            FadeImage.Alpha = 0.2f;
            FadeImage.ActiveEffectByName(nameof(FadeEffect));

            IsTransitioning = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            CurrentScreen.Draw(spriteBatch);
            if (IsTransitioning)
            {
                FadeImage.Draw(spriteBatch);
            }
        }

        public void LoadContent(string layout)
        {
            // "Design\\ScreenManager"
            Layout = layout;
            this.Populate();
            LoadContent();
        }

        public void LoadContent()
        {
            CurrentScreen.LoadContent();
        }

        public void UnloadContent()
        {
            CurrentScreen.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            CurrentScreen.Update(gameTime);
            ScreenTransition(gameTime);
        }

        private void ScreenTransition(GameTime gameTime)
        {
            if (IsTransitioning)
            {
                FadeImage.Update(gameTime);
                if (FadeImage.Alpha >= 0.99f)
                {
                    CurrentScreen.UnloadContent();
                    CurrentScreen = _nextScreen;
                    CurrentScreen.Populate();
                    _nextScreen.LoadContent();
                    ScreenType = CurrentScreen.GetType();
                    FadeImage.FadeEffect.Increase = false;
                    FadeImage.Alpha = 0.98f;
                }
                else if (FadeImage.Alpha <= 0.1f)
                {
                    FadeImage.IsActive = false;
                    IsTransitioning = false;
                }
            }
        }
    }
}
