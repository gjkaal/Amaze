// Rapbit Game development
//
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace NorthGame.Core.Abstractions
{
    public interface IScreenManager : IGameElement
    {
        Vector2 Dimensions { get; set; }

        ISprite Fader { get; }

        GraphicsDevice Graphics { get; set; }

        bool IsTransitioning { get; }

        Type ScreenType { get; }

        SpriteBatch Sprites { get; set; }

        Vector2 CenterOffset(Vector2 size);

        void ChangeScreen(string screen);

        void LoadContent(string layout);

        IGameScreen CurrentScreen { get; set; }

        Func<string, Type> ScreenResolver { set; }
    }
}
