using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Nodes;
using ParseKit.DOMElements._Classes.Lists;

namespace ParseKit.DOMSupport.DOMElements.Nodes
{
    interface IElement
    {
         string? namespaceURI { get;  }
         string? prefix { get;  }
         string localName { get;  }
         string tagName { get;  }

         string id { get; set; }
         string className { get; set; }
         DOMTokenList classList { get;  }

         Attr[] attributes { get;  }
         string? getAttribute(string name);
         string? getAttributeNS(string? @namespace, string localName);
         void setAttribute(string name, string value);
         void setAttributeNS(string? @namespace, string name, string value);
         void removeAttribute(string name);
         void removeAttributeNS(string? @namespace, string localName);
         bool hasAttribute(string name);
         bool hasAttributeNS(string? @namespace, string localName);

         HTMLCollection getElementsByTagName(string localName);
         HTMLCollection getElementsByTagNameNS(string? @namespace, string localName);
         HTMLCollection getElementsByClassName(string classNames);

        //DOM 4
         HTMLCollection children { get;  }
         Element? firstElementChild { get;  }
         Element? lastElementChild { get;  }
         Element? previousElementSibling { get;  }
         Element? nextElementSibling { get;  }
         long childElementCount { get;  }

        // NEW
         void prepend(Node nodes);
         void append(Node nodes);
         void before(Node nodes);
         void after(Node nodes);
         void replace(Node nodes);
         void prepend(string nodes);
         void append(string nodes);
         void before(string nodes);
         void after(string nodes);
         void replace(string nodes);
         void remove();
    }
}
