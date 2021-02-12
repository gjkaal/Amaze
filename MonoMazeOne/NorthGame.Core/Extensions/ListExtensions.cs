// Rapbit Game development
//
using Microsoft.Xna.Framework;
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

        public static void VisitMatrix(this Point size, Action<int, int> action)
        {
            // other set should be wall and is never active
            // this also prevents any reference calculation 
            // to trip.
            for (int x = 1; x < size.X-1; x++)
                for (int y = 1; y < size.Y-1; y++)
                    action.Invoke(x, y);
        }

    }


}
