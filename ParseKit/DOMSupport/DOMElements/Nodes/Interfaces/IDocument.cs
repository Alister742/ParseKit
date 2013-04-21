using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Nodes;
using ParseKit.DOMElements._Classes.Lists;
using ParseKit.DOMElements._Classes.Events;
using ParseKit.DOMElements._Classes.Ranges;
using ParseKit.DOMElements._Classes.Traversal;

namespace ParseKit.DOMSupport.DOMElements.Nodes
{
    interface IDocument
    {
         DOMImplementation implementation { get; private set; }
         string URL { get; private set; }
         string documentURI { get; private set; }
         string compatMode { get; private set; }
         string characterSet { get; private set; }
         string contentType { get; private set; }

         DocumentType? doctype { get; private set; }
         Element? documentElement { get; private set; }
         HTMLCollection getElementsByTagName(string localName);
         HTMLCollection getElementsByTagNameNS(string? @namespace, string localName);
         HTMLCollection getElementsByClassName(string classNames);
         Element? getElementById(string elementId);

         Element createElement(string localName);
         Element createElementNS(string? @namespace, string qualifiedName);
         DocumentFragment createDocumentFragment();
         Text createTextNode(string data);
         Comment createComment(string data);
         ProcessingInstruction createProcessingInstruction(string target, string data);

         Node importNode(Node node, bool deep = true);
         //Moving a Node, with methods such as Document.adoptNode, Node.appendChild, or Range.extractContents [DOM Range], must not cause the event listeners attached to it to be removed or un-registered.
         Node adoptNode(Node node);

         IEvent createEvent(string @interface);

         Range createRange();

        // NodeFilter.SHOW_ALL = 0xFFFFFFFF
         NodeIterator createNodeIterator(Node root, long whatToShow = 0xFFFFFFFF, NodeFilter? filter = null);
         TreeWalker createTreeWalker(Node root, long whatToShow = 0xFFFFFFFF, NodeFilter? filter = null);

        // NEW
        void prepend(Node nodes);
        void append(Node nodes);
        void prepend(string nodes);
        void append(string nodes);
    }
}
