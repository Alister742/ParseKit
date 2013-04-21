using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.Interfaces
{
    interface IDocument
    {
         DOMImplementation implementation { get;  }
         string URL { get;  }
         string documentURI { get; }
         string compatMode { get;  }
         string characterSet { get; }
         string contentType { get; }

         DocumentType doctype { get; }
         Element documentElement { get;  }
         HTMLCollection getElementsByTagName(string localName);
         HTMLCollection getElementsByTagNameNS(string @namespace, string localName);
         HTMLCollection getElementsByClassName(string classNames);
         Element getElementById(string elementId);

         Element createElement(string localName);
         Element createElementNS(string @namespace, string qualifiedName);
         DocumentFragment createDocumentFragment();
         Text createTextNode(string data);
         Comment createComment(string data);
         ProcessingInstruction createProcessingInstruction(string target, string data);

         Node importNode(Node node, bool deep = true);
         //Moving a Node, with methods such as Document.adoptNode, Node.appendChild, or Range.extractContents [DOM Range], must not cause the event listeners attached to it to be removed or un-registered.
         Node adoptNode(Node node);

         Event createEvent(string @interface);

         Range createRange();

        // NodeFilter.SHOW_ALL = 0xFFFFFFFF
         NodeIterator createNodeIterator(Node root, long whatToShow = 0xFFFFFFFF, NodeFilter filter = null);
         TreeWalker createTreeWalker(Node root, long whatToShow = 0xFFFFFFFF, NodeFilter filter = null);

        // NEW
        void prepend(Node nodes);
        void append(Node nodes);
        void prepend(string nodes);
        void append(string nodes);
    }
}
