// Rapbit Game development
//
using Microsoft.Xna.Framework;


namespace NorthGame.Core.Math
{
    public static class NorthMath
    {
        public static Rectangle Minkowski(int baseX, int baseY, Vector2 offset, Rectangle e1, Rectangle e2)
        {
            return new Rectangle(
                baseX + e1.X + e2.X + (int)offset.X,
                baseY + e1.Y + e2.Y + (int)offset.Y,
                e1.Width + e2.Width,
                e1.Height + e2.Height);
        }
    }
}
