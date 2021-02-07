// NorthGame Game development
//
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using NorthGame.Core.Extensions;
using NorthGame.Core.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace NorthGame.Core.Sound
{

    public sealed class SoundManager : ISoundManager
    {
        public string Layout { get; set; } = "Design\\Sounds";

        public readonly List<Music> Songs = new List<Music>();
        public readonly List<SoundFX> Sounds = new List<SoundFX>();
        private ContentManager _content;

        public void LoadContent(ContentManager content, string layout)
        {
            _content = new ContentManager(content.ServiceProvider, "Content");
            Layout = layout;
            LoadContent();
        }

        public void PlaySong(string name, float volume)
        {
            var song = Songs.FirstOrDefault(mbox => mbox.Key == name);
            if (song != null)
            {
                MediaPlayer.Play(song.Song());
                MediaPlayer.Volume = volume;
            }
        }

        public void PlaySfx(Sfx sfx)
        {
            var fx = Sounds.FirstOrDefault(mbox => mbox.Sfx == sfx);
            if (fx != null)
            {
                fx.Play(false);
            }
        }

        public void LoadContent()
        {
            var mgr = this;
            mgr.Populate(Layout);

            // load sound bites
            Sounds.Visit(m => m.LoadContent(_content));
            Songs.Visit(m => m.LoadContent(_content));
        }

        public void UnloadContent()
        {
            Sounds.Visit(m => m.UnloadContent());
            Songs.Visit(m => m.UnloadContent());
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // intentionally left empty
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
