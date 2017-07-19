using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arachnee.GraphElements
{
    [Flags]
    public enum ConnectionFlags
    {
        All = ~0,
        Actor = 1 << 0,
        Director = 1 << 1,
        UserRating = 1 << 2,
        MovieRating = 1 << 3
    }
}
