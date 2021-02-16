using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace NorthGame.Core.Abstractions
{
    public interface IMapLayer : IGameElement
    {
        LayerType ZPlane { get; }
        int Width { get; }
        int Height { get; }
        List<ITile> Tiles { get; }
        Point PlayerPosition { get; set; }

        void LoadContent(Point tileSize, IGameElementFactory tileFactory, ITileSet tileSet);
    }
}
