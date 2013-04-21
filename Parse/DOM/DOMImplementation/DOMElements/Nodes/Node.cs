using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.Core.Dom.DOMElements.Nodes;

namespace Parse.DOM.DOMElements
{
    enum NodeType
    {
         ELEMENT_NODE = 1,
         ATTRIBUTE_NODE = 2, // historical
         TEXT_NODE = 3,
         CDATA_SECTION_NODE = 4, // historical
         ENTITY_REFERENCE_NODE = 5, // historical
         ENTITY_NODE = 6, // historical
         PROCESSING_INSTRUCTION_NODE = 7,
         COMMENT_NODE = 8,
         DOCUMENT_NODE = 9,
         DOCUMENT_TYPE_NODE = 10,
         DOCUMENT_FRAGMENT_NODE = 11,
         NOTATION_NODE = 12, // historical
    }

    enum DocumentPosition
    {
         DOCUMENT_POSITION_DISCONNECTED = 0x01,
         DOCUMENT_POSITION_PRECEDING = 0x02,
         DOCUMENT_POSITION_FOLLOWING = 0x04,
         DOCUMENT_POSITION_CONTAINS = 0x08,
         DOCUMENT_POSITION_CONTAINED_BY = 0x10,
         DOCUMENT_POSITION_IMPLEMENTATION_SPECIFIC = 0x20,
    }

    public abstract class Node : EventTarget, ICloneable
    {
        Document asociatedDocument;
        StringBuilder contentBuilder = new StringBuilder();
        Node next;
        Node _lastChild;
        Node _parentNode;

        public bool IsText { get { return nodeType == (int)NodeType.TEXT_NODE || nodeType == (int)NodeType.CDATA_SECTION_NODE; } }

        public Node(Document doc)
        {
            ownerDocument = doc;
            _parentNode = doc;
            //if (_parentNode == null)
            //{
                
            //}
        }

        private void ReplaceAll()
        {
            throw new NotImplementedException();

            //The part of mutation algorithm
            //not supported yet
        }

        internal void NestTextNodes(Node prevNode, Node nextNode)
        {
            nextNode.parentNode = prevNode;
        }
        internal void UnnestTextNodes(Node prevNode, Node nextNode)
        {
            nextNode.parentNode = prevNode.parentNode;
        }
        internal bool AncestorNode(Node node)
        {
            Node n = this.parentNode;

            while (n != null && n != this)
            {
                if (n == node)
                    return true;
                n = n.parentNode;
            }
            return false;
        }
        private Node NormalizeWinner(Node firstNode, Node secondNode)
        {
            //first node has the priority
            if (firstNode == null)
                return secondNode;

            if (firstNode.nodeType == (int)NodeType.TEXT_NODE)
                return firstNode;
            if (secondNode.nodeType == (int)NodeType.TEXT_NODE)
                return secondNode;

            return null;
        } 

        #region Члены ICloneable

        public object Clone()
        {
            return cloneNode(true);
        }

        #endregion

        #region INode
        public short nodeType
        {
            get
            {
                if (this is Text)
                {
                    return (short)NodeType.TEXT_NODE;
                }
                if (this is ProcessingInstruction)
                {
                    return (short)NodeType.PROCESSING_INSTRUCTION_NODE;
                }
                if (this is Comment)
                {
                    return (short)NodeType.COMMENT_NODE;
                }
                if (this is Document)
                {
                    return (short)NodeType.DOCUMENT_NODE;
                }
                if (this is DocumentType)
                {
                    return (short)NodeType.DOCUMENT_TYPE_NODE;
                }
                if (this is DocumentFragment)
                {
                    return (short)NodeType.DOCUMENT_FRAGMENT_NODE;
                }

                return (short)NodeType.ELEMENT_NODE;
            }
        }
        public string nodeName
        {
            get
            {
                switch (nodeType)
                {
                    case (short)NodeType.TEXT_NODE:
                        return "#text";
                    case (short)NodeType.PROCESSING_INSTRUCTION_NODE:
                        return (this as ProcessingInstruction).target;
                    case (short)NodeType.COMMENT_NODE:
                        return "#comment";
                    case (short)NodeType.DOCUMENT_NODE:
                        return "#document";
                    case (short)NodeType.DOCUMENT_TYPE_NODE:
                        return (this as DocumentType).name;
                    case (short)NodeType.DOCUMENT_FRAGMENT_NODE:
                        return "#document-fragment";
                    default:
                        return (this as Element).tagName;
                }
            }
        }

        /// <summary>
        /// The baseURI attribute must return the associated base URL. WTF IS!
        /// </summary>
        public string baseURI { get; private set; }

        public Document ownerDocument
        {
            get
            {
                if (this is Document)
                {
                    return null;
                }
                return asociatedDocument;
            }
            set
            {
                asociatedDocument = value;
            }
        }

        /// <summary>
        /// The parentNode attribute must return the parent.
        /// </summary>
        public Node parentNode
        {
            get
            {
                if (_parentNode == null)
                    return null;

                if (_parentNode.nodeType != (int)NodeType.DOCUMENT_NODE)
                    return _parentNode;

                Node firstChild = _parentNode.firstChild;
                if (firstChild != null)
                {
                    Node node = firstChild;
                    do
                    {
                        if (node == this)
                        {
                            return _parentNode;
                        }
                        node = node.nextSibling;
                    }
                    while (node != null && node != firstChild);
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    _parentNode = ownerDocument;
                }
                else
                {
                    _parentNode = value;
                }
            }
        }
        /// <summary>
        /// The parentElement attribute must return the parent element.
        /// </summary>
        public Element parentElement { get; set; }

        public bool hasChildNodes()
        {
            return _lastChild != null;
        }
        /// <summary>
        /// The childNodes attribute must return a NodeList rooted at the context object matching only children.
        /// </summary>
        public NodeList childNodes { get { return new NodeList(this); } }

        public Node firstChild
        {
            get
            {
                //if (childNodes == null || childNodes.length == 0)
                //    return null;

                return _lastChild != null ? _lastChild.next : null;
            }
        }
        public Node lastChild
        {
            get
            {
                //if (childNodes == null || childNodes.length == 0)
                //    return null;

                return _lastChild == this ? null : _lastChild;
            }
            private set
            {
                _lastChild = value;
            }
        }

        /// <summary>
        /// The previousSibling attribute must return the previous sibling.
        /// 
        /// The previous sibling of an object is its first preceding sibling or null if it has no preceding sibling.
        /// 
        /// An object A is preceding an object B if A and B are in the same tree and A comes before B in tree order.
        /// 
        /// 
        /// </summary>
        public Node previousSibling
        {
            get
            {
                Node parent = parentNode;
                if (parent != null)
                {
                    Node node = parent.firstChild;
                    while (node != null)
                    {
                        Node nextSibling = node.nextSibling;
                        if (nextSibling == this)
                        {
                            break;
                        }
                        node = nextSibling;
                    }
                    return node;
                }
                return null;
            } 
        }

        /// <summary>
        /// The nextSibling attribute must return the next sibling.
        /// 
        /// The next sibling of an object is its first following sibling or null if it has no following sibling.
        /// 
        /// An object A is following an object B if A and B are in the same tree and A comes after B in tree order.
        /// </summary>
        public Node nextSibling
        {
            get
            {
                if (_parentNode != null && _parentNode.lastChild != this)
                    return next;

                return null;



                //Node parent = parentNode;
                //if (parent != null)
                //{
                //    if (next != parent.firstChild)
                //        return next;
                //}
                //return null;
            }
        }

        public string nodeValue
        {
            get
            {
                if (nodeType == (short)NodeType.TEXT_NODE ||
                    nodeType == (short)NodeType.COMMENT_NODE ||
                    nodeType == (short)NodeType.PROCESSING_INSTRUCTION_NODE)
                {
                    return (this as CharacterData).data;
                }

                return null;
            }
            set
            {
                if (nodeType == (short)NodeType.TEXT_NODE ||
                    nodeType == (short)NodeType.COMMENT_NODE ||
                    nodeType == (short)NodeType.PROCESSING_INSTRUCTION_NODE)
                {
                    (this as CharacterData).data = value;
                }
            }
        }
        public string textContent
        {
            get
            {
                if (nodeType == (short)NodeType.DOCUMENT_FRAGMENT_NODE ||
                    nodeType == (short)NodeType.ELEMENT_NODE)
                {
                    contentBuilder.Clear();
                    foreach (Node item in childNodes)
                    {
                        contentBuilder.Append(item.textContent);
                    }
                    return contentBuilder.ToString();
                }
                return nodeValue;
            }
            set
            {
                throw new NotImplementedException();

                if (value == null)
                    value = string.Empty;

                if (nodeType == (short)NodeType.DOCUMENT_FRAGMENT_NODE ||
                    nodeType == (short)NodeType.ELEMENT_NODE)
                {
                    if (value != string.Empty)
                    {
                        Node n = new Text(value, ownerDocument);
                    }
                    ReplaceAll();
                }
                nodeValue = value;
            }
        }

        public void normalize()
        {
            Node firstChildTextLikeNode = null;
            StringBuilder sb = new StringBuilder();
            for (Node crtChild = this.firstChild; crtChild != null; )
            {
                Node nextChild = crtChild.nextSibling;
                switch ((NodeType)crtChild.nodeType)
                {
                    case NodeType.TEXT_NODE:
                        {
                            sb.Append(crtChild.nodeValue);
                            Node winner = NormalizeWinner(firstChildTextLikeNode, crtChild);
                            if (winner == firstChildTextLikeNode)
                            {
                                this.removeChild(crtChild);
                            }
                            else
                            {
                                if (firstChildTextLikeNode != null)
                                    this.removeChild(firstChildTextLikeNode);
                                firstChildTextLikeNode = crtChild;
                            }
                            break;
                        }
                    case NodeType.ELEMENT_NODE:
                        {
                            crtChild.normalize();
                            goto default;
                        }
                    default:
                        {
                            if (firstChildTextLikeNode != null)
                            {
                                firstChildTextLikeNode.nodeValue = sb.ToString();
                                firstChildTextLikeNode = null;
                            }
                            sb.Remove(0, sb.Length);
                            break;
                        }
                }
                crtChild = nextChild;
            }
            if (firstChildTextLikeNode != null && sb.Length > 0)
                firstChildTextLikeNode.nodeValue = sb.ToString(); 
        }

        //Copying a Node, with methods such as Node.cloneNode or Range.cloneContents [DOM Range], must not copy the event listeners attached to it. Event listeners can be attached to the newly created Node afterwards, if so desired.
        public Node cloneNode(bool deep = true)
        {
            throw new NotImplementedException();
        }
        public bool isEqualNode(Node node)
        {
            throw new NotImplementedException();
        }

        public short compareDocumentPosition(Node other)
        {
            throw new NotImplementedException();
        }
        public bool contains(Node other)
        {
            if (childNodes == null)
                return false;

            for (int i = 0; i < childNodes.length; i++)
			{
			    if (childNodes[i].contains(other))
	            {
		            return true;
	            }
			}
            return false;
        }

        public string lookupPrefix(string nspace)
        {
            throw new NotImplementedException();
        }
        public string lookupNamespaceURI(string prefix)
        {
            throw new NotImplementedException();
        }
        public bool isDefaultNamespace(string nspace)
        {
            throw new NotImplementedException();
        }

        //NEW
        //not real supported, need queue mutation record
        public virtual Node insertBefore(Node newChild, Node refChild)
        {
            if (this == newChild || AncestorNode(newChild))
                throw new ArgumentException("The operation would yield an incorrect node tree.");

            if (refChild == null)
                return appendChild(newChild);

            if (refChild.parentNode != this)
                throw new ArgumentException("The operation would yield an incorrect node tree.");

            if (newChild == refChild)
                return newChild;

            Document childDoc = newChild.ownerDocument;
            Document thisDoc = ownerDocument;
            if (childDoc != null && childDoc != thisDoc && childDoc != this)
                throw new ArgumentException("The operation would yield an incorrect node tree.");

            if (newChild.parentNode != null)
                newChild.parentNode.removeChild(newChild);

            // special case for doc-fragment.
            if (newChild.nodeType == (int)NodeType.DOCUMENT_FRAGMENT_NODE)
            {
                Node first = newChild.firstChild;
                Node node = first;
                if (node != null)
                {
                    newChild.removeChild(node);
                    insertBefore(node, refChild);
                    // insert the rest of the children after this one.
                    InsertAfter(newChild, node);
                }
                return first;
            }

            Node newNode = newChild;
            Node refNode = refChild;

            string newChildValue = newChild.nodeValue;
            NodeChangedEventArgs args = ownerDocument.GetEventArgs(newChild, newChild.parentNode, this, newChildValue, newChildValue, NodeChangedAction.Insert);

            if (args != null)
                ownerDocument.BeforeEvent(args);

            if (refNode == firstChild)
            {
                newNode.next = refNode;
                lastChild.next = newNode;
                newNode.parentNode = this;

                if (newNode.IsText)
                {
                    if (refNode.IsText)
                    {
                        NestTextNodes(newNode, refNode);
                    }
                }
            }
            else
            {
                Node prevNode = refNode.previousSibling;

                newNode.next = refNode;
                prevNode.next = newNode;
                newNode.parentNode  = this;

                if (prevNode.IsText)
                {
                    if (newNode.IsText)
                    {
                        NestTextNodes(prevNode, newNode);
                        if (refNode.IsText)
                        {
                            NestTextNodes(newNode, refNode);
                        }
                    }
                    else
                    {
                        if (refNode.IsText)
                        {
                            UnnestTextNodes(prevNode, refNode);
                        }
                    }
                }
                else
                {
                    if (newNode.IsText)
                    {
                        if (refNode.IsText)
                        {
                            NestTextNodes(newNode, refNode);
                        }
                    }
                }
            }

            if (args != null)
                ownerDocument.AfterEvent(args);

            return newNode;
        } 

        //public Node insertBefore(Node node, Node child) //child CAN BE NULL
        //{
        //    if (child != null && child.parentNode != this)
        //        throw new ArgumentException("Node not contains such child");

        //    if (node == null)
        //        throw new ArgumentNullException();

        //    Node parent = child != null ?  child.parentNode : null;

        //    if (node.contains(parent))
        //        throw new DOMError("HierarchyRequestError");

        //    NodeType type = (NodeType)node.nodeType;

        //    if (type != NodeType.DOCUMENT_NODE ||
        //        type != NodeType.DOCUMENT_FRAGMENT_NODE ||
        //        type != NodeType.ELEMENT_NODE ||
        //        type != NodeType.DOCUMENT_TYPE_NODE ||
        //        type != NodeType.TEXT_NODE ||
        //        type != NodeType.PROCESSING_INSTRUCTION_NODE ||
        //        type != NodeType.COMMENT_NODE)
        //        throw new DOMError("HierarchyRequestError");

        //    if (type == NodeType.DOCUMENT_TYPE_NODE && type != NodeType.DOCUMENT_NODE)
        //        throw new DOMError("HierarchyRequestError");

        //    if (parent.nodeType == (short)NodeType.DOCUMENT_NODE)
        //    {
        //        if (type == NodeType.DOCUMENT_FRAGMENT_NODE)
        //        {
        //            /*
        //            //If node has more than one element child or has a Text node child, throw a "HierarchyRequestError" and terminate these steps.
        //            if (node.childNodes.length > 1 || node.childNodes.Any((n) => { return n.nodeType == (short)NodeType.TEXT_NODE  true : false; }))
        //                throw new DOMError("HierarchyRequestError");
        //             */
        //            /*
        //            If node has one element child, and parent has an element child, child is a doctype, or child is not null and a doctype is following child, throw a "HierarchyRequestError" and terminate these steps.
        //             */
        //        }
        //        /*
        //        If node is an element, and parent has an element child, child is a doctype, or child is not null and a doctype is following child, throw a "HierarchyRequestError" and terminate these steps. 
        //        If node is a doctype and either parent has a doctype child, an element is preceding child, or child is null and parent has an element child, throw a "HierarchyRequestError" and terminate these steps.
        //         */
        //    }

        //    node.ownerDocument = this.ownerDocument;
        //    if (node.childNodes != null)
        //    {
        //        for (int i = 0; i < node.childNodes.length; i++)
        //        {
        //            childNodes[i].ownerDocument = asociatedDocument;
        //        }
        //    }

        //    Node previous = child != null ? child.previousSibling : null;
        //    node.previousSibling = previous;

        //    if (child != null) child.previousSibling = node;

        //    node.nextSibling = child;

        //    int index = childNodes.IndexOf(child);
        //    index = index != -1 ? index : childNodes.Count - 1;

        //    childNodes.Insert(index, node);

        //    return node;
        //}

        public virtual Node InsertAfter(Node newChild, Node refChild)
        {
            if (this == newChild || AncestorNode(newChild))
                throw new ArgumentException("Node not contains such child");

            if (refChild == null)
                return PrependChild(newChild);

            if (refChild.parentNode != this)
                throw new ArgumentException("Node not contains such child");

            if (newChild == refChild)
                return newChild;

            Document childDoc = newChild.ownerDocument;
            Document thisDoc = ownerDocument;
            if (childDoc != null && childDoc != thisDoc && childDoc != this)
                throw new ArgumentException("Node not contains such child");

            if (newChild.parentNode != null)
                newChild.parentNode.removeChild(newChild);

            // special case for doc-fragment.
            if (newChild.nodeType == (int)NodeType.DOCUMENT_FRAGMENT_NODE)
            {
                Node last = refChild;
                Node first = newChild.firstChild;
                Node node = first;
                while (node != null)
                {
                    Node next = node.nextSibling;
                    newChild.removeChild(node);
                    InsertAfter(node, last);
                    last = node;
                    node = next;
                }
                return first;
            }

            Node newNode = (Node)newChild;
            Node refNode = (Node)refChild;

            string newChildValue = newChild.nodeValue;
            NodeChangedEventArgs args = ownerDocument.GetEventArgs(newChild, newChild.parentNode, this, newChildValue, newChildValue, NodeChangedAction.Insert);

            if (args != null)
                ownerDocument.BeforeEvent(args);

            if (refNode == lastChild)
            {
                newNode.next = refNode.next;
                refNode.next = newNode;
                lastChild = newNode;
                newNode.parentNode = this;

                if (refNode.IsText)
                {
                    if (newNode.IsText)
                    {
                        NestTextNodes(refNode, newNode);
                    }
                }
            }
            else
            {
                Node nextNode = refNode.next;

                newNode.next = nextNode;
                refNode.next = newNode;
                newNode.parentNode = this;;

                if (refNode.IsText)
                {
                    if (newNode.IsText)
                    {
                        NestTextNodes(refNode, newNode);
                        if (nextNode.IsText)
                        {
                            NestTextNodes(newNode, nextNode);
                        }
                    }
                    else
                    {
                        if (nextNode.IsText)
                        {
                            UnnestTextNodes(refNode, nextNode);
                        }
                    }
                }
                else
                {
                    if (newNode.IsText)
                    {
                        if (nextNode.IsText)
                        {
                            NestTextNodes(newNode, nextNode);
                        }
                    }
                }
            }


            if (args != null)
                ownerDocument.AfterEvent(args);

            return newNode;
        }

        public virtual Node PrependChild(Node newChild)
        {
            return insertBefore(newChild, firstChild);
        } 

        //Moving a Node, with methods such as Document.adoptNode, Node.appendChild, or Range.extractContents [DOM Range], must not cause the event listeners attached to it to be removed or un-registered.
        public Node appendChild(Node newChild)
        {
            Document thisDoc = ownerDocument;
            if (thisDoc == null)
            {
                thisDoc = this as Document;
            }

            //if (newChild.parentNode != this)
            //    throw new ArgumentException("The operation would yield an incorrect node tree.");

            if (this == newChild || AncestorNode(newChild))
                throw new ArgumentException("The operation would yield an incorrect node tree.");

            Document childDoc = newChild.ownerDocument;
            if (childDoc != null && childDoc != thisDoc && childDoc != this)
                throw new ArgumentException("The operation would yield an incorrect node tree.");

            if (newChild.parentNode != null)
                newChild.parentNode.removeChild(newChild);

            // special case for doc-fragment.
            if (newChild.nodeType == (int)NodeType.DOCUMENT_FRAGMENT_NODE)
            {
                Node first = newChild.firstChild;
                Node node = first;
                while (node != null)
                {
                    Node next = node.nextSibling;
                    newChild.removeChild(node);
                    appendChild(node);
                    node = next;
                }
                return first;
            }

            string newChildValue = newChild.nodeValue;
            NodeChangedEventArgs args = thisDoc.GetEventArgs(newChild, newChild.parentNode, this, newChildValue, newChildValue, NodeChangedAction.Insert);

            if (args != null)
                thisDoc.BeforeEvent(args);

            Node refNode = lastChild;
            Node newNode = newChild;

            if (refNode == null)
            {
                newNode.next = newNode;
                lastChild = newNode;
                newNode.parentNode = this;
            }
            else
            {
                newNode.next = refNode.next;
                refNode.next = newNode;
                lastChild = newNode;
                newNode.parentNode = this;

                if (refNode.IsText && newNode.IsText)
                {
                    NestTextNodes(refNode, newNode);
                }
            }

            if (args != null)
                thisDoc.AfterEvent(args);

            return newNode;
        }

        public virtual Node replaceChild(Node newChild, Node oldChild)
        {
            Node nextNode = oldChild.nextSibling;
            removeChild(oldChild);
            Node node = insertBefore(newChild, nextNode);
            return oldChild;
        }

        public virtual Node removeChild(Node oldChild)
        {
            if (oldChild.parentNode != this)
                throw new ArgumentException("The operation would yield an incorrect node tree.");

            Node oldNode = (Node)oldChild;

            string oldNodeValue = oldNode.nodeValue;
            NodeChangedEventArgs args = ownerDocument.GetEventArgs(oldNode, this, null, oldNodeValue, oldNodeValue, NodeChangedAction.Remove);

            if (args != null)
                ownerDocument.BeforeEvent(args);

            Node lastNode = lastChild;

            if (oldNode == firstChild)
            {
                if (oldNode == lastNode)
                {
                    lastChild = null;
                    oldNode.next = null;
                    oldNode.parentNode = null;
                }
                else
                {
                    Node nextNode = oldNode.next;

                    if (nextNode.IsText)
                    {
                        if (oldNode.IsText)
                        {
                            UnnestTextNodes(oldNode, nextNode);
                        }
                    }

                    lastNode.next = nextNode;
                    oldNode.next = null;
                    oldNode.parentNode = null;
                }
            }
            else
            {
                if (oldNode == lastNode)
                {
                    Node prevNode = oldNode.previousSibling;
                    prevNode.next = oldNode.next;
                    lastChild = prevNode;
                    oldNode.next = null;
                    oldNode.parentNode = null;
                }
                else
                {
                    Node prevNode = oldNode.previousSibling;
                    Node nextNode = oldNode.next;

                    if (nextNode.IsText)
                    {
                        if (prevNode.IsText)
                        {
                            NestTextNodes(prevNode, nextNode);
                        }
                        else
                        {
                            if (oldNode.IsText)
                            {
                                UnnestTextNodes(oldNode, nextNode);
                            }
                        }
                    }

                    prevNode.next = nextNode;
                    oldNode.next = null;
                    oldNode.parentNode = null;
                }
            }

            if (args != null)
                ownerDocument.AfterEvent(args);

            return oldChild;
        } 
        #endregion

        #region HtmlAPI

        public string innerHTML
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public string outerHTML
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    };
}
