using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.Interfaces
{
    interface ITreeWalker
    {
        Node root { get; }
        long whatToShow { get; }
        NodeFilter filter { get; }
        Node currentNode { get; set; }

        Node parentNode();
        Node firstChild();
        Node lastChild();
        Node previousSibling();
        Node nextSibling();
        Node previousNode();
        Node nextNode();
    }
}
