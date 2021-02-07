// Rapbit Game development
//
using Microsoft.Xna.Framework.Content;

namespace NorthGame.Core.Abstractions
{

    public interface ISoundManager : IGameElement
    {
        void PlaySfx(Sfx sfx);

        void PlaySong(string name, float volume);

        void LoadContent(ContentManager contentManager, string layout);
    }
}
