// Rapbit Game development
//
using Microsoft.Xna.Framework;
using NorthGame.Core.Abstractions;

namespace NorthGame.Core.Game
{
    public class SpriteSheetEffect : ImageEffect, ISpriteSheetEffect
    {
        // set frames and current to contain the number of images in
        // x and y size within the image. Current points to the
        // frame that is in use.
        public int Current { get; set; } = 0;

        /// <summary>
        /// Select the frame to use, shouold fall within the FrameBegin-FrameEnd range.
        /// </summary>
        public int FrameInactive { get; set; } = 0;

        public int FrameBegin { get; set; } = 0;
        public int FrameEnd { get; set; } = 4;
        public Point Frames { get; set; } = new Point(2, 2);
        private int _frameCounter = 0;
        public int SwitchFrame { get; set; } = 100;
        public Point FrameSize { get; set; } = new Point(32, 32);

        public SpriteSheetEffect()
        {
        }

        public override void LoadContent(ISprite image)
        {
            base.LoadContent(image);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Image.IsActive)
            {
                _frameCounter += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_frameCounter >= SwitchFrame)
                {
                    _frameCounter = 0;
                    Current += 1;
                    if (Current > FrameEnd) Current = FrameBegin;
                }
            }
            else
            {
                Current = FrameInactive;
            }
            var positionX = (Current % Frames.X);
            var positionY = (Current / Frames.X);
            Image.SourceRect = new Rectangle(
                positionX * FrameSize.X,
                positionY * FrameSize.Y,
                FrameSize.X,
                FrameSize.Y);
        }
    }
}
