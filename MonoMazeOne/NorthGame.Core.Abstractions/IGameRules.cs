namespace NorthGame.Core.Abstractions
{

    public interface IGameRules
    {
        void ApplyGameRules(Direction nextMove);
        void RestartMap(IMapLayer mapLayer);
    }
}