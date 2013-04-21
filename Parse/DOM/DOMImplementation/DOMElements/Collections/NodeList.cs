using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ParseKit.Core.Dom.DOMElements.Interfaces;
using ParseKit.Core.Dom.DOMElements.Nodes;

namespace Parse.DOM.DOMElements
{
    class NodeListEnumerator : IEnumerator
    {
        Node parent;
        Node current;
        bool isFirst;

        public NodeListEnumerator(Node parent)
        {
            this.parent = parent;
            this.current = parent.firstChild;
            isFirst = true;
        }

        #region Члены IEnumerator

        object IEnumerator.Current { get { return this.Current; } }

        public Node Current { get { return current; } }

        public bool MoveNext()
        {
            if (isFirst)
            {
                isFirst = false;
            }
            else if (current != null)
            {
                current = current.nextSibling;
            }

            return current != null;
        }

        public void Reset()
        {
            current = parent.firstChild;
            isFirst = true;
        }

        #endregion
    }

    public class NodeList : IEnumerable, INodeList
    {
        Node parent;

        public NodeList(Node parent)
        {
            this.parent = parent;
        }

        public virtual Node this[int i] { get { return item(i); } }

        //private Node NextElemInPreOrder(Node curNode)
        //{
        //    //For preorder walking, first try its child
        //    Node retNode = curNode.firstChild;
        //    if (retNode == null)
        //    {
        //        //if no child, the next node forward will the be the NextSibling of the first ancestor which has NextSibling 
        //        //so, first while-loop find out such an ancestor (until no more ancestor or the ancestor is the rootNode
        //        retNode = curNode;
        //        while (retNode != null
        //                && retNode != root
        //                && retNode.nextSibling == null)
        //        {
        //            retNode = retNode.parentNode;
        //        }
        //        //then if such ancestor exists, set the retNode to its NextSibling
        //        if (retNode != null && retNode != root)
        //            retNode = retNode.nextSibling;
        //    }
        //    if (retNode == root)
        //        //if reach the rootNode, consider having walked through the whole tree and no more element after the curNode
        //        retNode = null;
        //    return retNode;
        //}

        //private Node PrevElemInPreOrder(Node curNode)
        //{
        //    //For preorder walking, the previous node will be the right-most node in the tree of PreviousSibling of the curNode 
        //    Node retNode = curNode.previousSibling;
        //    // so if the PreviousSibling is not null, going through the tree down to find the right-most node 
        //    while (retNode != null)
        //    {
        //        if (retNode.lastChild == null)
        //            break;
        //        retNode = retNode.lastChild;
        //    }
        //    // if no PreviousSibling, the previous node will be the curNode's parentNode 
        //    if (retNode == null)
        //        retNode = curNode.parentNode;
        //    // if the final retNode is rootNode, consider having walked through the tree and no more previous node 
        //    if (retNode == root)
        //        retNode = null;
        //    return retNode;
        //}

        //private Node GetNthMatchingNode(Node n, bool bForward, int nDiff)
        //{
        //    Node node = n;
        //    for (int ind = 0; ind < nDiff; ind++)
        //    {
        //        node = GetMatchingNode(node, bForward);
        //        if (node == null)
        //            return null;
        //    }
        //    return node;
        //}

        //private Node GetMatchingNode(Node node, bool bForward)
        //{
        //    if (bForward)
        //    {
        //        return NextElemInPreOrder(node);
        //    }
        //    else
        //    {
        //        return PrevElemInPreOrder(node);
        //    }
        //}

        //internal void ConcurrencyCheck(NodeChangedEventArgs args)
        //{
        //    if (args.Node == current)
        //    {
        //        changeCount++;
        //        this.curInd = -1;
        //        current = root;
        //    }
        //    count = -1;
        //}

        //public Node GetNextNode(Node n)
        //{
        //    if (n == null)
        //        n = root;

        //    return NextElemInPreOrder(n);
        //} 

        #region Interface NodeList

        public Node item(int index)
        {
            if (index < 0)
                throw new IndexOutOfRangeException("index value cant be lower zero");

            foreach (Node item in this)
            {
                if (index == 0)
                {
                    return item;
                }
                index--;
            }
            return null;
        }

        public int length
        {
            get
            {
                int i = 0;
                foreach (Node n in this)
                {
                    i++;
                }
                return i;
            }
        }

        #endregion

        #region Члены IEnumerable

        public IEnumerator GetEnumerator()
        {
            return new NodeListEnumerator(parent);
        }

        #endregion

        internal bool Contains(Node node)
        {
            foreach (var item in this)
            {
                if (node == item)
                {
                    return true;
                }
            }
            return false;
        }

        internal int IndexOf(Element element)
        {
            int index = 0;
            foreach (var item in this)
            {
                if (element == item)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }
    };
}
