using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.Interfaces
{
    interface IParentNode
    {
        HTMLCollection children { get; }
        Element firstElementChild { get; }
        Element lastElementChild { get; }
        long childElementCount { get; }

        //NEW (SUPPORTED!)
        void prepend(Node nodes);
        void append(Node nodes);
        void prepend(string nodes);
        void append(string nodes);
    }
}
