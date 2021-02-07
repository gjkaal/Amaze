// Rapbit Game development
//
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NorthGame.Core.Events;
using NorthGame.Core.Abstractions;
using NorthGame.Core.Extensions;
using System;
using NorthGame.Core.Menu;

namespace NorthGame.Core
{
    public class MenuManager : IMenuManager
    {
        private readonly IInputManager _input;
        private readonly IScreenManager _screen;
        private readonly ContentManager _content;
        public bool IsTransitioning { get; private set; }
        public string Layout { get; set; }
        public IMenuDefinition Menu { get; private set; }

        public MenuManager(ContentManager content, IInputManager inputManager, IScreenManager screenManager)
        {
            _content = content;
            _input = inputManager;
            _screen = screenManager;
            Menu = new MenuDefinition(inputManager, screenManager);
            Menu.OnMenuChange += MenuChanged;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Menu.Draw(spriteBatch);
        }

        public void LoadContent()
        {
            LoadContent(Layout);
        }

        public void LoadContent(string menu)
        {
            if (!string.IsNullOrEmpty(menu))
            {
                Menu.Id = menu;
            }
        }

        public void UnloadContent()
        {
            Menu.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            if (!IsTransitioning)
            {
                Menu.Update(gameTime);
                if (_input.EnterKeyPressed())
                {
                    var nextId = Menu.LinkId();
                    var linkType = Menu.LinkType();
                    if (linkType == Constants.Screen )
                    {
                        // transition in screen
                        _screen.ChangeScreen(nextId);
                    }
                    else if (linkType == Constants.Menu)
                    {
                        // transition in menu
                        IsTransitioning = true;
                        Menu.Transition(0.9f);
                    }
                }
            }
            Transition(gameTime);
        }

        private void MenuChanged(object sender, EventArgs e)
        {
            if (!(e is MenuChangedEventArgs menuChanges)) return;
            Menu.UnloadContent();

            Menu.Populate($"{menuChanges.NewValue}");
            Menu.LoadContent();

            Menu.Transition(1.0f);
        }

        private void Transition(GameTime gametime)
        {
            // TODO: Can this be optimized?
            if (IsTransitioning)
            {
                for (var i = 0; i < Menu.Items.Count; i++)
                {
                    float first = Menu.Items[0].Image.Alpha;
                    float last = Menu.Items[Menu.Items.Count - 1].Image.Alpha;

                    Menu.Items[i].Image.Update(gametime);
                    if (first == 0.0f && last == 0.0f)
                    {
                        Menu.Id = Menu.LinkId();
                    }
                    else if (first == 1.0f && last == 1.0f)
                    {
                        IsTransitioning = false;
                    }
                }
            }
        }
    }
}
