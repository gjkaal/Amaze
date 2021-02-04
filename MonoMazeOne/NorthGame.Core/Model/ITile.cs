// Rapbit Game development
//
using Microsoft.Xna.Framework;

namespace NorthGame.Core.Model
{
    public interface ITile : IGameElement
    {
        TileState TileState { get; set; }
        bool Active { get; set; }
        Vector2 Position { get; set; }
        Rectangle SourceRect { get; set; }      
    }
}
