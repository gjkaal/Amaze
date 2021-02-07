using Microsoft.Xna.Framework;

namespace NorthGame.Core.Abstractions
{

    public interface IFadeEffect : IImageEffect
    {
        bool Increase { get; set; }
    }
}