﻿// Rapbit Game development
//
using Microsoft.Xna.Framework;

namespace NorthGame.Core.Model
{
    public interface IImageEffect
    {
        void LoadContent(ISprite image);

        void UnloadContent();

        void Update(GameTime gameTime);

        bool IsActive { get; set; }
    }

    public interface ISpriteSheetEffect : IImageEffect { }
}
