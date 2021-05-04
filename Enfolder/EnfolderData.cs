using System;
using System.Collections.Generic;

namespace Enfolder
{
    public static class EnfolderData
    {
        public static event EventHandler<EnfolderDataEventArgs> OnItemAdded;

        public static void AddItems(List<string> items)
        {
            OnItemAdded?.Invoke(null, new EnfolderDataEventArgs(items));
        }
    }
}
