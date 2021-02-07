// Rapbit Game development
//
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NorthGame.Core.Abstractions;
using NorthGame.Core.ContainerService;

namespace NorthGame.Core.Game
{
    public abstract class GameScreenBase : IGameElement, IGameScreen
    {
        protected ContentManager Content;
        protected readonly IInputManager Input;
        protected readonly IScreenManager Screen;

        public static IMenuManager MenuManager => NorthGameContainer.Instance.Resolve<IMenuManager>();

        private SpriteFont _statusFont;
        virtual public string Layout { get; set; }
        private bool _slow;
        private int _loopCount;

        protected string Status { get; set; }

        protected GameScreenBase()
        {
            Input = NorthGameContainer.Instance.Resolve<IInputManager>();
            Screen = NorthGameContainer.Instance.Resolve<IScreenManager>();
            Content = NorthGameContainer.Instance.Resolve<ContentManager>();
        }

        public virtual void LoadContent()
        {
            _statusFont = Content.Load<SpriteFont>("Fonts/Verdana");
        }

        public virtual void UnloadContent()
        {
            Content?.Unload();
        }

        public virtual void Update(GameTime gameTime)
        {
            _loopCount++;
            Input.Update(gameTime);
            _slow = gameTime.IsRunningSlowly;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_statusFont, $"{(_slow ? '!' : ' ')}{_loopCount:D8} : {Status}", new Vector2(10, (int)Screen.Dimensions.Y - 20), Color.White);
        }
    }
}
