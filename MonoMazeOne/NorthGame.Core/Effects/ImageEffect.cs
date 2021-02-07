// Rapbit Game development
//
using Microsoft.Xna.Framework;
using NorthGame.Core.Abstractions;

namespace NorthGame.Core.Game
{
    public abstract class ImageEffect : IImageEffect
    {
        protected ISprite Image { get; private set; }

        /// <summary>
        /// If true, then this effect is activated.
        /// </summary>
        public bool IsActive { get; set; } = false;

        protected ImageEffect()
        {
        }

        public virtual void LoadContent(ISprite image)
        {
            Image = image;
        }

        public virtual void UnloadContent()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }
    }
}
