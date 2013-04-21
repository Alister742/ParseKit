using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.Core.Dom.Interfaces;

namespace Parse.DOM.DOMElements
{
    public class TreeWalker : DomWalker, ITreeWalker
    {
        public TreeWalker(Node root, long whatToShow, NodeFilter filter)
            : base(root, whatToShow, filter)
        {
            this.currentNode = root;
        }

        public Node currentNode { get; set; }

        public Node parentNode()
        {
            throw new NotImplementedException();
        }
        public Node firstChild()
        {
            throw new NotImplementedException();
        }
        public Node lastChild()
        {
            throw new NotImplementedException();
        }
        public Node previousSibling()
        {
            throw new NotImplementedException();
        }
        public Node nextSibling()
        {
            throw new NotImplementedException();
        }
        public Node previousNode()
        {
            throw new NotImplementedException();
        }
        public Node nextNode()
        {
            throw new NotImplementedException();
        }
    };
}
