using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arachnee.GraphElements
{
    public class Connection
    {
        public string Left { get; set; }
        public string Right { get; set; }

        public ConnectionFlags Flags { get; set; }

        public string Id { get; set; }
        
        public bool Contains(string entryId)
        {
            return Left == entryId || Right == entryId;
        }

        public string GetOppositeOf(string entryId)
        {
            if (Left == entryId)
            {
                return Right;
            }

            if (Right == entryId)
            {
                return Left;
            }

            throw new ArgumentException("Invalid id", "entryId");
        }
    }
}
