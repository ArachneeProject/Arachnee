using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arachnee.GraphElements
{
    public class DefaultEntry : Entry
    {
        private static DefaultEntry _singleton;

        public static DefaultEntry Instance
        {
            get { return _singleton ?? (_singleton = new DefaultEntry()); }
        }

        private DefaultEntry()
        {
        }
    }
}
