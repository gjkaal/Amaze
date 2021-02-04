using Microsoft.Xna.Framework;

namespace NorthGame.Core.Model
{

    public interface IFadeEffect : IImageEffect
    {
        bool Increase { get; set; }
    }
}