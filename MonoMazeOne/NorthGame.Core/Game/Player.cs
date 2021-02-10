// Rapbit Game development
//
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NorthGame.Core.Abstractions;
using NorthGame.Core.Extensions;

namespace NorthGame.Core.Game
{
    public class Player : IGameElement, IPlayer
    {
        //---
        private readonly IInputManager _inputManager;
        public string Name { get; private set; }
        public int Index { get; private set; }
        private Point _tileDimensions;
        public Point TileSpan { get; private set; }
        public Rectangle HitBox { get; set; }
        public Sprite Image { get; } = new Sprite();
        public string Layout { get; set; } = "Design\\Player\\Player1";
        public IMapLayer MapLayer { get; set; }
        public float MoveSpeed { get; set; } = 0.0f;
        public int State { get; private set; } = 0;
        public Direction MoveDirection { get; private set; }

        public Point TileDimension
        {
            get
            {
                return _tileDimensions;
            }
            set
            {
                _tileDimensions = value;
                TileOffset = new Vector2(_tileDimensions.X / 2, _tileDimensions.Y / 2);
                TileSpan = new Point(HitBox.Width / _tileDimensions.X, HitBox.Height / _tileDimensions.Y);
            }
        }

        public Vector2 TileOffset { get; private set; }
        public Vector2 Velocity { get; set; } = Vector2.Zero;
        public LayerType ZPlane { get; set; } = LayerType.Collision;
        
        private Player()
        {
            // No initialization without parameters.
        }
        public Player(int index, string name, IInputManager inputManager)
        {
            // TODO initialize with key settings / controller etc.
            Index = index;
            Name = name;
            _inputManager = inputManager;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
        }

        public Point GridReference()
        {
            var playerPosition = PlayerPosition();
            return new Point((int)(playerPosition.X / TileDimension.X), (int)(playerPosition.Y / TileDimension.Y));
        }

        public void LoadContent(string layout)
        {
            Layout = layout;
            LoadContent();
        }

        public void LoadContent()
        {
            this.Populate();
            Image.LoadContent();
        }

        public Vector2 PlayerPosition() => Position() + TileOffset;

        public Vector2 Position()
        {
            return Image.Position;
        }

        public void UnloadContent()
        {
            Image.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            Velocity = Vector2.Zero;

            // TODO: Compound movement (x and y)
            // TODO: Gravity (buoyancy, tracks etc.)
            if (Velocity.X == 0)
            {
                if (_inputManager.KeyDown(Keys.Down))
                {
                    MoveDirection = Direction.Down;
                    Velocity = new Vector2(0, MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                else if (_inputManager.KeyDown(Keys.Up))
                {
                    MoveDirection = Direction.Up;
                    Velocity = new Vector2(0, -MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
            }

            if (Velocity.Y == 0)
            {
                if (_inputManager.KeyDown(Keys.Left))
                {
                    MoveDirection = Direction.Left;
                    Velocity = new Vector2(-MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                }
                else if (_inputManager.KeyDown(Keys.Right))
                {
                    MoveDirection = Direction.Right;
                    Velocity = new Vector2(MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                }
            }

            // TODO : check if moving within game field
           

            // Trigger effects by calling image update
            Image.Update(gameTime);

        }

        void SetPlayerPositionOutsideTile(ITile tile, Rectangle tileHitbox)
        {
            if (MoveDirection==Direction.Up)
            {
                // Set the position outside the hitbox
                // The dimension that is not changing (e.g. X for up-down)
                // is rounded to the nearest integer, to prevent immediate 
                // collision when starting to move in another direction.
                Image.Position = new Vector2(
                     (int)Image.Position.X,
                     tile.Position.Y + TileSpan.Y * tileHitbox.Height );
                return;
            }
            if (MoveDirection == Direction.Down)
            {
                Image.Position = new Vector2(
                    (int)Image.Position.X,
                    tile.Position.Y - TileSpan.Y * TileDimension.Y );
                return;
            }
            if (MoveDirection == Direction.Left)
            {
                Image.Position = new Vector2(
                    tile.Position.X + TileSpan.X * tileHitbox.Width ,
                    (int)Image.Position.Y);
                return;
            }
            if (MoveDirection == Direction.Right)
            {
                Image.Position = new Vector2(
                    tile.Position.X - TileSpan.X * TileDimension.X ,
                    (int)Image.Position.Y);
                return;
            }
        }
    }
}
