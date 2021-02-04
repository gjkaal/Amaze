// Rapbit Game development
//
using System;
using System.Collections.Generic;

namespace NorthGame.Core.Extensions
{

    public static class StringExtensions
    {
        public static string RemoveNameSpace(this string item)
        {
            var n = item.LastIndexOf('.');
            if (n < 0) return item;
            return item.Substring(n + 1);
        }

        public static string RemoveString(this string item, string remove)
        {
            RuntimeAssert.NotNull(item, remove);
            var n = item.IndexOf(remove);
            if (n < 0) return item;
            return item.Substring(n + remove.Length);
        }
    }
}
