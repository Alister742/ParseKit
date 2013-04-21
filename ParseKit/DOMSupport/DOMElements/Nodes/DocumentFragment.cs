using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParseKit.DOMElements._Classes.Nodes
{
    class DocumentFragment : Node
    {
        //NEW
        void prepend(Node nodes);
        void append(Node nodes);
        void prepend(string nodes);
        void append(string nodes);
    };
}
