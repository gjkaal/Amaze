// Rapbit Game development
//
using Microsoft.Xna.Framework;
using NorthGame.Core.Abstractions;

namespace NorthGame.Core.Game
{
    public class FadeEffect : ImageEffect, IFadeEffect
    {
        public float FadeSpeed { get; set; } = Constants.DefaultFadeSpeed;
        public bool Increase { get; set; } = false;

        // TODO: Implement fadre options to just increase, just decrease and cycle
        // TODO: Add readonly flags for AtMaximum, AtMinimum
        // TODO: Add eventhook for DirectionChange

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Image.IsActive)
            {
                if (!Increase)
                {
                    Image.Alpha -= FadeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    Image.Alpha += FadeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (Image.Alpha <= 0.0f)
                {
                    Increase = true;
                    Image.Alpha = 0.0f;
                }
                else if (Image.Alpha > 1.0f)
                {
                    Increase = false;
                    Image.Alpha = 1.0f;
                }
            }
            else
            {
                Image.Alpha = 1.0f;
            }
        }
    }
}
