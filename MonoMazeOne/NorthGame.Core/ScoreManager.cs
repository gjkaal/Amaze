// Rapbit Game development
//
using NorthGame.Core.Abstractions;

namespace NorthGame.Core
{
    public sealed class ScoreManager : IScoreManager
    {
        public int Lost { get; set; }
        public int Found { get; set; }

        public void AddScore(int playerIndex, int value)
        {
           // left empty
        }

        public void Reset()
        {
            Lost = 0;
            Found = 0;
        }
    }
}
