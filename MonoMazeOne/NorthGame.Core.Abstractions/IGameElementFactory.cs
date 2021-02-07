namespace NorthGame.Core.Abstractions
{
    public interface IGameElementFactory
    {
        ITile CreateTile();
        IPlayer CreatePlayer(int index, string name);
        ISprite CreateSprite(string resourcePath);
    }
}