using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Ranges;
using ParseKit.DOMElements._Classes.Traversal;
using ParseKit.DOMElements._Classes.Lists;
using ParseKit.DOMElements._Classes.Events;
using ParseKit.DOMSupport.DOMElements.Nodes;
using ParseKit.DOMSupport.DOMElements.Events;

namespace ParseKit.DOMElements._Classes.Nodes
{
    class Document : Node, IDocument, IDocumentEvent
    {
        public DOMImplementation implementation { get; private set; }
        public string URL { get; private set; }
        public string documentURI { get; private set; }
        public string compatMode { get; private set; }
        public string characterSet { get; private set; }
        public string contentType { get; private set; }

        public DocumentType? doctype { get; private set; }
        public Element? documentElement { get; private set; }
        public HTMLCollection getElementsByTagName(string localName);
        public HTMLCollection getElementsByTagNameNS(string? @namespace, string localName);
        public HTMLCollection getElementsByClassName(string classNames);
        public Element? getElementById(string elementId);

        public Element createElement(string localName);
        public Element createElementNS(string? @namespace, string qualifiedName);
        public DocumentFragment createDocumentFragment();
        public Text createTextNode(string data);
        public Comment createComment(string data)
        {
            return new Comment(data);
        }
        public ProcessingInstruction createProcessingInstruction(string target, string data);

        public Node importNode(Node node, bool deep = true);
        public Node adoptNode(Node node);

        public IEvent createEvent(string eventType)
        {
            return EventFactory.CreateEventByType(eventType);
        }

        public Range createRange();

        // NodeFilter.SHOW_ALL = 0xFFFFFFFF
        public NodeIterator createNodeIterator(Node root, long whatToShow = 0xFFFFFFFF, NodeFilter? filter = null);
        public TreeWalker createTreeWalker(Node root, long whatToShow = 0xFFFFFFFF, NodeFilter? filter = null);

        // NEW
        void prepend(Node nodes);
        void append(Node nodes);
        void prepend(string nodes);
        void append(string nodes);
    };

    class XMLDocument : Document { };
}