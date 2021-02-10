using Microsoft.Xna.Framework;

namespace NorthGame.Core.Abstractions
{
    public interface IMapLayer : IGameElement
    {
        LayerType ZPlane { get; }

        void LoadContent(Point tileSize, IGameElementFactory tileFactory, ITileSet tileSet, bool colisionMap);
    }
}
