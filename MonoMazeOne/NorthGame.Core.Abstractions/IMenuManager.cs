// Rapbit Game development
//

namespace NorthGame.Core.Abstractions
{
    public interface IMenuManager : IGameElement
    {
        IMenuDefinition Menu { get; }

        bool IsTransitioning { get; }

        void LoadContent(string menuLayout);
    }
}
