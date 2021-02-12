// Rapbit Game development
//
using Microsoft.Xna.Framework;

namespace NorthGame.Core.Abstractions
{
    public interface ITile : IGameElement
    {
        bool Active { get; set; }
        TileState TileState { get; set; }
        Vector2 Position { get; set; }
    }
}
