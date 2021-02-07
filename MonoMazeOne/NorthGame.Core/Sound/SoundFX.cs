// NorthGame Game development
//
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using NorthGame.Core.Abstractions;
using NorthGame.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NorthGame.Core.Sound
{
    public class Music
    {
        public string Key { get; set; }
        public string ResourcePath { get; set; }
        public float DefaultVolume { get; set; }
        public bool Looped { get; set; }
        private Song _song;

        public Song Song() => _song;

        public void LoadContent(ContentManager content)
        {
            _song = content.Load<Song>(ResourcePath);
        }

        public void UnloadContent()
        {
            _song.Dispose();
        }
    }

    public class SoundFX : ISoundEffect
    {
        public string Key { get; set; }
        public string ResourcePath { get; set; }
        public float DefaultVolume { get; set; }
        public float DefaultPitch { get; set; }
        public int Count { get; set; }
        public bool Looped { get; set; }
        public Sfx Sfx { get; private set; }

        private SoundEffect _soundEffect;
        private readonly List<SoundEffectInstance> _soundInstance = new List<SoundEffectInstance>();

        public void LoadContent(ContentManager content)
        {
            if (Enum.TryParse<Sfx>(Key, out var sfxKey)) Sfx = sfxKey;
            _soundEffect = content.Load<SoundEffect>(ResourcePath);
            _soundInstance.Clear();
            for (var i = 0; i < Count; i++)
            {
                var newInstance = _soundEffect.CreateInstance();
                newInstance.IsLooped = Looped;
                _soundInstance.Add(newInstance);
            }
        }

        public void UnloadContent()
        {
            _soundInstance.Visit((m) =>
            {
                m.Stop();
                m.Dispose();
            });
            _soundInstance.Clear();
            _soundEffect.Dispose();
        }

        public void Play(bool allowInterupt)
        {
            var instance = _soundInstance.FirstOrDefault(m => m.State == SoundState.Stopped);
            if (instance == null && allowInterupt)
            {
                // TODO: Round robin for next
                instance = _soundInstance.FirstOrDefault();
                instance.Stop();
                instance.Play();
            }
            else if (instance != null)
            {
                instance.Play();
            }
            // no instance available
        }

        public void Silence()
        {
            _soundInstance.Visit(m => m.Stop());
        }
    }
}
