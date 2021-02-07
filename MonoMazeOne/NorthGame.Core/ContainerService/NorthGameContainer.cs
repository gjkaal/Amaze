// Rapbit Game development
//
using NorthGame.Core.Abstractions;
using System;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using NorthGame.Core.Sound;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace NorthGame.Core.ContainerService
{

    public class NorthGameContainer : INorthGameContainer
    {
        public static NorthGameContainer Instance => _instance;

        private readonly Container _container = new Container();
        private readonly static NorthGameContainer _instance = new NorthGameContainer();
        private NorthGameContainer()
        {
           _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
        }

        public T Resolve<T>() where T : class
        {
            return _container.GetInstance<T>();
        }

        public IGameScreen ResolveScreen(Type screenType)
        {
            return (IGameScreen)_container.GetInstance(screenType);
        }

        public void SetUp(Action<Container> setupContainer, Func< INorthGameConfiguration> configuration)
        {
            // TODO : Add functional logger
            _container.Register( configuration, Lifestyle.Singleton);
            _container.Register<ILogger>(() => NullLogger.Instance, Lifestyle.Singleton);

            _container.Register<IInputManager, InputManager>(Lifestyle.Singleton);
            _container.Register<IScreenManager, ScreenManager>(Lifestyle.Singleton);
            // 
            _container.Register<IMenuManager, MenuManager>();
            _container.Register<ISoundManager, SoundManager>();

            // Game continues with container setup
            setupContainer.Invoke(_container);

            _container.Verify();
        }

        public T GetService<T>()
        {
            //   return (T)_container.GetInstance(typeof(T));
            return default;
        }

    }
}
