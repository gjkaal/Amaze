// Rapbit Game development
//
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NorthGame.Core.Abstractions;

namespace NorthGame.Core
{

    public sealed class InputManager : IInputManager
    {
        public KeyboardState Keyboard { get; private set; }
        private KeyboardState _previous;
        private readonly IScreenManager _screen;

        public InputManager(IScreenManager screen)
        {
            _screen = screen;
        }

        public void Update(GameTime gameTime)
        {
            _previous = Keyboard;
            if (!_screen.IsTransitioning)
            {
                Keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            }
        }

        public bool KeyPressed(params Keys[] keys)
        {
            if (Keyboard.GetPressedKeyCount() == 0) return false;
            foreach (var k in keys)
            {
                if (Keyboard.IsKeyDown(k) && _previous.IsKeyUp(k)) return true;
            }
            return false;
        }

        public bool KeyRelease(params Keys[] keys)
        {
            foreach (var k in keys)
            {
                if (Keyboard.IsKeyUp(k) && _previous.IsKeyDown(k)) return true;
            }
            return false;
        }

        public bool KeyDown(params Keys[] keys)
        {
            if (Keyboard.GetPressedKeyCount() == 0) return false;
            foreach (var k in keys)
            {
                if (Keyboard.IsKeyDown(k)) return true;
            }
            return false;
        }

        public int MoveLeftRight()
        {
            return KeyPressed(Keys.Right) ? 1
                : KeyPressed(Keys.Left) ? -1 : 0;
        }

        public int MoveTopDown()
        {
            return KeyPressed(Keys.Up) ? -1
                : KeyPressed(Keys.Down) ? +1 : 0;
        }

        public bool EnterKeyPressed()
        {
            return KeyPressed(Keys.Enter);
        }
    }
}
