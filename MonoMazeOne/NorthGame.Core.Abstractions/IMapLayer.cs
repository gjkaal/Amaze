using Microsoft.Xna.Framework;

namespace NorthGame.Core.Abstractions
{
    public interface IMapLayer : IGameElement
    {
        LayerType ZPlane { get; }

        bool CheckTileCollision(IPlayer player, out ITile tile, out Rectangle hitBox);

        void LoadContent(Point tileSize, IGameElementFactory tileFactory, ITileSet tileSet);
    }
}
