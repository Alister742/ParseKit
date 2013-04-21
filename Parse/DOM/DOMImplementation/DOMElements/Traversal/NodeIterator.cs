using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.Core.Dom.Interfaces;

namespace Parse.DOM.DOMElements
{
    //what is this class for? and what he do?
    public class NodeIterator : DomWalker, INodeIterator
    {
        public NodeIterator(Node root, long whatToShow, NodeFilter filter)
            : base(root, whatToShow, filter)
        {
            this.referenceNode = root;
            this.pointerBeforeReferenceNode = true;
        }

        #region INodeIterator

        public Node referenceNode { get; private set; }
        public bool pointerBeforeReferenceNode { get; private set; }

        public Node nextNode()
        {
            return Traverse(true);
        }
        public Node previousNode()
        {
            return Traverse(false);
        }

        public void detach()
        {
            //The detach() method must do nothing.
        }

        #endregion

        private Node Traverse(bool directionNext)
        {
            Node next = null;
            bool beforeNode = pointerBeforeReferenceNode;

            while (true)
            {
                if (directionNext)
                {
                    if (beforeNode)
                    {
                        beforeNode = false;
                    }
                    else
                    {
                        next = referenceNode.nextSibling;
                        if (next == null)
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    if (beforeNode)
                    {
                        next = referenceNode.previousSibling;
                        if (next == null)
                        {
                            return null;
                        }
                    }
                    else
                    {
                        beforeNode = true;
                    }
                }

                int result;
                try
                {
                    result = base.Filter(next);
                }
                catch (Exception)
                {
                    return null;
                }

                if (result == FilterResult.FILTER_ACCEPT)
                {
                    break;
                }
            }

            pointerBeforeReferenceNode = beforeNode;
            referenceNode = next;
            return next;
        }
    };
}
