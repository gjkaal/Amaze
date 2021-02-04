// Rapbit Game development
//
using System;
using System.Collections.Generic;

namespace NorthGame.Core.Extensions
{
    public static class ListExtensions
    {
        public static void Visit<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items) action.Invoke(item);
        }
    }
}
