// Rapbit Game development
//
using Microsoft.Xna.Framework.Content;

namespace NorthGame.Core.Model
{
    public interface ISoundEffect
    {
        void LoadContent(ContentManager content);

        void Play(bool allowInterupt);

        void Silence();
    }
}
