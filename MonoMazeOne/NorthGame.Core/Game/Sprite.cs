// Rapbit Game development
//

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NorthGame.Core.Abstractions;
using NorthGame.Core.ContainerService;
using NorthGame.Core.Extensions;
using System;

namespace NorthGame.Core.Game
{
    /// <summary>
    /// Base class for image manipulation in the game
    /// </summary>
    public class Sprite : EffectBase, ISprite
    {
        public Sprite()
        {
        }

        /// <summary>
        /// Image is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Resource path
        /// </summary>
        public string ResourcePath { get; set; } = string.Empty;

        /// <summary>
        /// Alpha for th eimage
        /// </summary>
        public float Alpha { get; set; } = 1.0f;

        /// <summary>
        /// Test
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Fot to use for the text
        /// </summary>
        public string FontName { get; set; } = "Fonts/Verdana";

        /// <summary>
        /// Texture for the background.
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// Size of the image to display.
        /// </summary>
        public Rectangle SourceRect { get; set; } = Rectangle.Empty;

        /// <summary>
        /// Position for the element.
        /// </summary>
        public Vector2 Position { get; set; } = Vector2.Zero;

        public Vector2 Scale { get; set; } = Vector2.One;
        public Vector2 Size { get; set; } = Vector2.Zero;

        public string Layout { get; set; }

        private RenderTarget2D _target;
        private Vector2 _origin;
        private ContentManager _content;
        private SpriteFont _font;

        /// <summary>
        /// Draw the image on the active viewscreen.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            var viewScale = Scale;
            if (Size!= Vector2.Zero)
            {
                var xScale = Size.X / SourceRect.Width;
                var yScale = Size.Y / SourceRect.Height;
                viewScale = new Vector2(xScale, yScale);
            }

            _origin = new Vector2(SourceRect.Width / 2, SourceRect.Height / 2);
            spriteBatch.Draw(Texture,
                Position + _origin,
                SourceRect,
                Color.White * Alpha, 0.0f,
                _origin,
                viewScale,
                SpriteEffects.None, 0.0f);
        }      

        public void LoadContent()
        {
            base.LoadEffects();

            var dim = Vector2.Zero;
            _content = NorthGameContainer.Instance.Resolve<ContentManager>();
            _font = _content.Load<SpriteFont>(FontName);

            if (!string.IsNullOrEmpty(ResourcePath))
            {
                Texture = _content.Load<Texture2D>(ResourcePath);
            }

            if (Texture != null)
            {
                dim.X += Texture.Width + _font.MeasureString(Text).X;
                dim.Y += System.Math.Max(Texture.Height, _font.MeasureString(Text).Y);
            }
            else
            {
                dim.Y = _font.MeasureString(Text).Y;
                dim.X = _font.MeasureString(Text).X;
            }

            if (SourceRect.IsEmpty)
            {
                SourceRect = new Rectangle(0, 0, (int)dim.X, (int)dim.Y);
            }

            _target = new RenderTarget2D(ScreenManager.Graphics, (int)dim.X, (int)dim.Y);
            ScreenManager.Graphics.SetRenderTarget(_target);
            ScreenManager.Graphics.Clear(Color.Transparent);
            ScreenManager.Sprites.Begin();
            if (Texture != null) ScreenManager.Sprites.Draw(Texture, Vector2.Zero, Color.White);
            if (!string.IsNullOrEmpty(Text))
            {
                ScreenManager.Sprites.DrawString(_font, Text, Vector2.Zero, Color.White);
            }
            ScreenManager.Sprites.End();

            Texture = _target;
            ScreenManager.Graphics.SetRenderTarget(null);

            if (!string.IsNullOrEmpty(Effects))
            {
                Effects.Split(';').Visit((e) => ActiveEffectByName(e));
            }
        }

        public void UnloadContent()
        {
            _content.Unload();
            UnloadEffects();
        }

        public new void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public Vector2 GetSize()
        {
            return new Vector2(SourceRect.Width, SourceRect.Height);
        }

        public void ActiveEffect<T>() where T : IImageEffect, new()
        {
            ActiveEffect<T>((e) => e.LoadContent(this));
        }

        public void ActiveEffectByName(string effect)
        {
            ActiveEffectByName(effect, (e) => e.LoadContent(this));
        }


    }
}
