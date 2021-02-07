// Rapbit Game development
//
using System;
using System.Collections.Generic;

namespace NorthGame.Core.Abstractions
{

    public interface IMenuDefinition : IGameElement
    {
        string Id { get; set; }
        event EventHandler OnMenuChange;

        char Axis { get;  }
        string Effects { get; set; }
        int ItemNumber { get; }
        void ChangeMenu();
        void Transition(float alpha);
        string LinkId();
        string LinkType();
        List<IMenuItem> Items { get; }
    }
}
