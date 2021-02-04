// Rapbit Game development
//
using System;

namespace NorthGame.Core.Extensions
{
    public static class RuntimeAssert
    {
        public static void NotNull(params object[] items)
        {
#if DEBUG
            // for production, this method is empty
            foreach (var item in items)
            {
                if (item == null) throw new ArgumentNullException();
                var stringItem = item as string;
                if (string.IsNullOrEmpty(stringItem))
                {
                    throw new ArgumentNullException();
                }
            }
#endif
        }
    }
}
