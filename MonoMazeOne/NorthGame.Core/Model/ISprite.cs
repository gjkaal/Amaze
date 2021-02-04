// Rapbit Game development
//
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NorthGame.Core.Model
{
    public interface IEffectable
    {
        IFadeEffect FadeEffect { get; }
        ISpriteSheetEffect SpriteSheetEffect { get; }

        string Effects { get; set; }

        void DeactivateEffect(string effect);
    }

    public interface ISprite : IGameElement, IEffectable
    {
        float Alpha { get; set; }
        string FontName { get; set; }
        bool IsActive { get; set; }
        string ResourcePath { get; set; }
        Vector2 Position { get; set; }
        Vector2 Scale { get; set; }
        Vector2 Size { get; set; }

        Vector2 GetSize();

        Rectangle SourceRect { get; set; }
        string Text { get; set; }
        Texture2D Texture { get; set; }

        void ActiveEffect<T>() where T : IImageEffect, new();

        void ActiveEffectByName(string effect);
    }
}
