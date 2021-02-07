// Rapbit Game development
//
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NorthGame.Core.Abstractions
{
    public interface IGameElement
    {
        string Layout { get; set; }

        void LoadContent();

        void UnloadContent();

        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch);

    }
}
