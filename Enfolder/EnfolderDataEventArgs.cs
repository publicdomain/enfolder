using System;
using System.Collections.Generic;

namespace Enfolder
{
    public class EnfolderDataEventArgs : EventArgs
    {
        public EnfolderDataEventArgs(List<string> items)
        {
            this.Items = items;
        }

        public List<string> Items { get; }
    }
}
