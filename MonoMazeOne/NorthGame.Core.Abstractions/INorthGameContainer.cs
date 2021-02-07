// Rapbit Game development
//
using System;

namespace NorthGame.Core.Abstractions
{
    public interface INorthGameContainer
    {
        T Resolve<T>() where T : class;
        IGameScreen ResolveScreen(Type screenType);
    }
}
