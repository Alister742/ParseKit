using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Nodes;

namespace ParseKit.DOMElements._Classes.Traversal
{
    class TreeWalker
    {
        public Node root { get; private set; }
        public long whatToShow { get; private set; }
        public NodeFilter? filter { get; private set; }
        Node currentNode { get; set; }

        public Node? parentNode();
        public Node? firstChild();
        public Node? lastChild();
        public Node? previousSibling();
        public Node? nextSibling();
        public Node? previousNode();
        public Node? nextNode();
    };
}
