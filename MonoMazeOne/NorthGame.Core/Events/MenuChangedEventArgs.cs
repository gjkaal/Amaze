// Rapbit Game development
//
using System;

namespace NorthGame.Core.Events
{
    public class MenuChangedEventArgs : EventArgs
    {
        public MenuChangedEventArgs(string oldValue, string newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public string OldValue { get; }
        public string NewValue { get; }
    }
}
