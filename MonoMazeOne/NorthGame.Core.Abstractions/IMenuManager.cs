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

    public interface IScoreManager
    {
        int Lost { get; set; }
        int Found { get; set; }
        void AddScore(int playerIndex, int value);
        void Reset();
    }

}
