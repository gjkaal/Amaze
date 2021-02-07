// Rapbit Game development
//

using NorthGame.Core.Abstractions;
using NorthGame.Core.Game;

namespace NorthGame.Core
{
    public class GameElementFactory : IGameElementFactory
    {
        private readonly IInputManager _inputManager;
        public GameElementFactory(IInputManager inputManager)
        {
            _inputManager = inputManager;
        }

        public virtual IPlayer CreatePlayer(int index, string name)
        {
            return new Player(index, name, _inputManager);
        }

        public virtual ISprite CreateSprite(string resourcePath)
        {
            return new Sprite
            {
                ResourcePath = resourcePath
             };
        }

        public virtual ITile CreateTile()
        {
            return new Tile();
        }
    }
}
