using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Events;
using ParseKit.DOMElements._Classes.Lists;

namespace ParseKit.DOMElements._Classes.Nodes
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

    abstract class Node : EventTarget
    {
        Document _asociatedDocument;
        StringBuilder _contentBuilder = new StringBuilder();

        public Node(Document doc)
        {
            ownerDocument = doc;
        }
        private void ReplaceAll()
        {
            throw new NotImplementedException();

            //The part of mutation algorithm
            //not supported yet
        }


        #region DOM Node interface implementation
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
        /// The baseURI attribute must return the associated base URL. WTF IS?!
        /// </summary>
        public string? baseURI { get; private set; }

        public Document ownerDocument
        {
            get
            {
                if (this is Document)
                {
                    return null;
                }
                return _asociatedDocument;
            }
            set
            {
                _asociatedDocument = value;
            }
        }

        /// <summary>
        /// The parentNode attribute must return the parent.
        /// </summary>
        public Node? parentNode { get; set; }
        /// <summary>
        /// The parentElement attribute must return the parent element.
        /// </summary>
        public Element? parentElement { get; set; }

        public bool hasChildNodes()
        {
            return childNodes != null && childNodes.length > 0;
        }
        /// <summary>
        /// The childNodes attribute must return a NodeList rooted at the context object matching only children.
        /// </summary>
        public NodeList childNodes { get; private set; }

        public Node? firstChild
        {
            get
            {
                if (childNodes == null || childNodes.length == 0)
                    return null;

                return childNodes.First();
            }
        }
        public Node? lastChild
        {
            get
            {
                if (childNodes == null || childNodes.length == 0)
                    return null;

                return childNodes.Last();
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
                throw new NotImplementedException();
            }
            private set
            {
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }
            private set
            {
                throw new NotImplementedException();
            }
        }

        public abstract string nodeValue
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
        public abstract string textContent
        {
            get
            {
                if (nodeType == (short)NodeType.DOCUMENT_FRAGMENT_NODE ||
                    nodeType == (short)NodeType.ELEMENT_NODE)
                {
                    _contentBuilder.Clear();
                    foreach (var item in childNodes)
                    {
                        _contentBuilder.Append(item.textContent);
                    }
                    return _contentBuilder.ToString();
                }
                return nodeValue;
            }
            set
            {
                if (value == null)
                    value = string.Empty;

                if (nodeType == (short)NodeType.DOCUMENT_FRAGMENT_NODE ||
                    nodeType == (short)NodeType.ELEMENT_NODE)
                {
                    if (value != string.Empty)
                    {
                        Node n = new Text() { data = value };
                    }
                    ReplaceAll();
                }
                nodeValue = value;
            }
        }


        public abstract void normalize()
        {
            //An object A is called a descendant of an object B, if either A is a child of B or A is a child of an object C that is a descendant of B.

            List<int> removeAt = new List<int>();

            for (int i = 0; i < childNodes.length; i++)
			{

                throw new NotImplementedException();
                throw new NotImplementedException();

                if (childNodes[i] is Text)
                {
                    Text node = childNodes[i] as Text;
                    int length = (int)node.length;

                    if (length == 0)
                    {
                        //REMOVE() METHOD FROM MUTATION ALGORITHM
                        throw new NotImplementedException();
                        //childNodes[i] = null; 
                        continue;
                    }

                    /////CONTINUE FROM THIS LINE..

                    node.replaceData(0, 0, node.wholeText);

                    Node curNode = node.nextSibling;

                    while (curNode is Text)
                    {
                        
                    }
                }
            }

            childNodes.RemoveNulls();


            //Let data be the concatenation of the data of node's contiguous Text nodes (excluding itself), in tree order. 


            //Replace data with node node, offset length, count 0, and data data. 


            //Let current node be node's next sibling. 


            //While current node is a Text node: 


            //For each range whose start node is current node, add length to its start offset and set its start node to node. 


            //For each range whose end node is current node, add length to its end offset and set its end node to node. 


            //For each range whose start node is current node's parent and start offset is current node's index, set its start node to node and its start offset to length. 


            //For each range whose end node is current node's parent and end offset is current node's index, set its end node to node and its end offset to length. 


            //Add current node's length attribute value to length. 


            //Set current node to its next sibling. 


            //Remove node's contiguous Text nodes (excluding itself), in tree order.
        }

        //Copying a Node, with methods such as Node.cloneNode or Range.cloneContents [DOM Range], must not copy the event listeners attached to it. Event listeners can be attached to the newly created Node afterwards, if so desired.
        public Node cloneNode(bool deep = true);
        public bool isEqualNode(Node? node);


        public short compareDocumentPosition(Node other);
        public bool contains(Node? other);

        public string? lookupPrefix(string? @namespace);
        public string? lookupNamespaceURI(string? prefix);
        public bool isDefaultNamespace(string? @namespace);

        //NEW
        public Node insertBefore(Node node, Node? child);
        //Moving a Node, with methods such as Document.adoptNode, Node.appendChild, or Range.extractContents [DOM Range], must not cause the event listeners attached to it to be removed or un-registered.
        public Node appendChild(Node node);
        public Node replaceChild(Node node, Node child);
        public Node removeChild(Node child);
        #endregion
    };
}
