// Rapbit Game development
//
using Microsoft.Xna.Framework;
using NorthGame.Core.Abstractions;
using NorthGame.Core.ContainerService;
using NorthGame.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NorthGame.Core.Game
{
    public class EffectBase : IEffectable
    {
        protected readonly IScreenManager ScreenManager;
        private readonly Dictionary<string, IImageEffect> _effects = new Dictionary<string, IImageEffect>();

        public EffectBase()
        {
            ScreenManager = NorthGameContainer.Instance.Resolve<IScreenManager>();
        }

        public IFadeEffect FadeEffect => _e1.Value;
        private readonly Lazy<IFadeEffect> _e1 = new Lazy<IFadeEffect>(() => new FadeEffect());

        public ISpriteSheetEffect SpriteSheetEffect => _e2.Value;
        private readonly Lazy<SpriteSheetEffect> _e2 = new Lazy<SpriteSheetEffect>(() => new SpriteSheetEffect());

        private void SetEffect<T>(ref T effect) where T : IImageEffect
        {
            if (effect == null)
            {
                effect = (T)Activator.CreateInstance(typeof(T));
            }
            else
            {
                var image = this as ISprite;
                effect.IsActive = true;
                effect.LoadContent(image);
            }

            // Add or replace
            var keyName = typeof(T).Name;
            if (_effects.ContainsKey(keyName))
            {
                _effects.Remove(keyName);
            }
            _effects.Add(keyName, effect);
        }

        protected void ActiveEffect<T>(Action<IImageEffect> loadContent) where T : IImageEffect, new()
        {
            ActiveEffectByName(typeof(T).Name, loadContent);
        }

        protected void ActiveEffectByName(string effect, Action<IImageEffect> loadContent)
        {
            if (!_effects.ContainsKey(effect))
            {
                if ("FadeEffect" == effect)
                {
                    var item = FadeEffect;
                    SetEffect(ref item);
                    return;
                }

                if ("SpriteSheetEffect" == effect)
                {
                    var item = SpriteSheetEffect;
                    SetEffect(ref item);
                    return;
                }
                return;
            }
            _effects[effect].IsActive = true;
            loadContent(_effects[effect]);
        }

        public void DeactivateEffect(string effect)
        {
            if (!_effects.ContainsKey(effect)) return;
            _effects[effect].IsActive = false;
            _effects[effect].UnloadContent();
        }

        /// <summary>
        /// Effects collection.
        /// </summary>
        public string Effects { get; set; }

        protected void LoadEffects()
        {
            // Set effects
            if (string.IsNullOrEmpty(Effects)) return;
            var e = Effects.Split(';');
            if (e.Contains(nameof(FadeEffect)))
            {
                var effect = FadeEffect;
                SetEffect(ref effect);
            }

            if (e.Contains(nameof(SpriteSheetEffect)))
            {
                var effect = SpriteSheetEffect;
                SetEffect(ref effect);
            }
        }

        protected void UnloadEffects()
        {
            _effects.Values.Visit((e) => e.UnloadContent());
        }

        protected void Update(GameTime gameTime)
        {
            _effects.Values.Where(m => m.IsActive).Visit((e) => e.Update(gameTime));
        }
    }
}
