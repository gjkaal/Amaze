// Rapbit Game development
//
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using NorthGame.Core.Abstractions;
using NorthGame.Core.Events;
using NorthGame.Core.Extensions;

namespace NorthGame.Core.Menu
{
    public class MenuDefinition : IMenuDefinition
    {
        public event EventHandler OnMenuChange;

        public char Axis { get; set; }
        public string Effects { get; set; }
        public int ItemNumber { get; private set; }

        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (value == _id) return;
                var oldValue = _id;
                _id = value;
                OnMenuChange(this, new MenuChangedEventArgs(oldValue, value));
            }
        }

        private readonly IInputManager _input;
        private readonly IScreenManager _screen;

        public MenuDefinition(IInputManager inputManager, IScreenManager screenManager)
        {
            _input = inputManager;
            _screen = screenManager;

            _id = string.Empty;
            ItemNumber = 0;
            Effects = string.Empty;
            Axis = 'Y';
        }

        public void ChangeMenu()
        {
        }

        public void Transition(float alpha)
        {
            foreach (var item in Items)
            {
                item.Image.IsActive = true;
                item.Image.Alpha = alpha;
                if (alpha == 0.0f)
                {
                    item.Image.FadeEffect.Increase = true;
                }
                else
                {
                    item.Image.FadeEffect.Increase = false;
                }
            }
        }

        public string LinkId()
        {
            return Items[ItemNumber].LinkId;
        }

        public string LinkType()
        {
            return Items[ItemNumber].LinkType;
        }

        private void AlignMenuItems()
        {
            Vector2 dimensions = Vector2.Zero;
            Items.Visit(i => dimensions += i.Image.GetSize());

            var offset = _screen.CenterOffset(dimensions);
            var screenWidth = _screen.Dimensions.X;
            var screenHeight = _screen.Dimensions.Y;

            // Center align menu items.
            if (Axis == 'X') Items.Visit(item =>
            {
                item.Image.Position = new Vector2(offset.X, (screenWidth - item.Image.SourceRect.Height) / 2);
                offset += item.Image.GetSize();
            });
            if (Axis == 'Y') Items.Visit(item =>
            {
                item.Image.Position = new Vector2((screenHeight - item.Image.SourceRect.Width) / 2, offset.Y);
                offset += item.Image.GetSize();
            });
        }


        private string _id;

        public List<IMenuItem> Items { get; } = new List<IMenuItem>();
        public string Layout { get; set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            Items.Visit(m => m.Image.Draw(spriteBatch));
        }

        public void LoadContent()
        {
            var split = Effects.Split(';');
            foreach (var i in Items)
            {
                i.Image.LoadContent();
                foreach (var s in split)
                {
                    i.Image.ActiveEffectByName(s);
                }
            }
            AlignMenuItems();
        }

        public void UnloadContent()
        {
            Items.Visit(item => item.Image.UnloadContent());
            Items.Clear();
        }

        public void Update(GameTime gameTime)
        {
            var im = _input;
            if (Axis == 'X')
            {
                ItemNumber += im.MoveLeftRight();
            }
            else if (Axis == 'Y')
            {
                ItemNumber += im.MoveTopDown();
            }

            if (ItemNumber < 0) ItemNumber = 0;
            if (ItemNumber > Items.Count - 1) ItemNumber = Items.Count - 1;

            for (var i = 0; i < Items.Count; i++)
            {
                if (i == ItemNumber)
                {
                    Items[i].Image.IsActive = true;
                }
                else
                {
                    Items[i].Image.IsActive = false;
                }
                Items[i].Image.Update(gameTime);
            }
        }
    }
}
