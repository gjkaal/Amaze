// Rapbit Game development
//
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NorthGame.Core.Abstractions;
using System;

namespace NorthGame.Core.Game
{

    public class Tile : ITile
    {
        public Vector2 Position { get; set; }
        public Rectangle SourceRect { get; set; }
        public TileState TileState { get; set; }
        public string Layout { get; set; }
        public bool Active { get; set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            throw new NotSupportedException();
        }

        public void LoadContent()
        {
        }

        public void UnloadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
