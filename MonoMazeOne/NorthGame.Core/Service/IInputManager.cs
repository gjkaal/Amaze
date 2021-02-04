// Rapbit Game development
//
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace NorthGame.Core.Service
{
    public interface IInputManager
    {
        void Update(GameTime gameTime);

        KeyboardState Keyboard { get; }

        bool KeyPressed(params Keys[] keys);

        bool KeyRelease(params Keys[] keys);

        bool KeyDown(params Keys[] keys);

        int MoveLeftRight();

        int MoveTopDown();

        bool EnterKeyPressed();
    }
}
