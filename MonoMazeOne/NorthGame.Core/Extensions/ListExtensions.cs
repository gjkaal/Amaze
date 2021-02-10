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
            for (int x = 0; x < size.X; x++)
                for (int y = 0; y < size.Y; y++)
                    action.Invoke(x, y);
        }

        public static void VisitMatrixReverseY(this Point size, Action<int, int> action)
        {
            for (int x = 0; x < size.X; x++)
                for (int y = size.Y-1; y >=0; y--)
                    action.Invoke(x, y);
        }
    }


}
