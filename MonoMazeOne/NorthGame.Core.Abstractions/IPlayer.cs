// Rapbit Game development
//
using Microsoft.Xna.Framework;

namespace NorthGame.Core.Abstractions
{
    public interface IPlayer : IGameElement
    {
        Rectangle HitBox { get; }
        IMapLayer MapLayer { get; set; }
        Vector2 Velocity { get; set; }
        LayerType ZPlane { get; set; }

        Point GridReference();

        void LoadContent(string layout);

        Vector2 Position();
        Vector2 PlayerPosition();
        Point TileDimension { get; set; }

        /// <summary>
        /// Size of the player sprite in tiles
        /// </summary>
        Point TileSpan { get; }

        string Name { get; }
        int Index { get; }
    }
}
