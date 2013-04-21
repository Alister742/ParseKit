using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Lists;
using ParseKit.DOMSupport.DOMElements.Nodes;

namespace ParseKit.DOMElements._Classes.Nodes
{
    class Element : Node, IElement
    {
        public bool TryGetAttribyte(string name, out Attr attr)
        {
            for (int i = 0; i < attributes.Length; i++)
            {
                if (attributes[i].name.NoncaseEqual(name))
                {
                    attr = attributes[i];
                    return true;
                }
            }
            attr = null;
            return false;
        }

        #region IElement
        public string? namespaceURI { get; private set; }
        public string? prefix { get; private set; }
        public string localName { get; private set; }
        public string tagName { get; private set; }

        public string id { get; set; }
        public string className { get; set; }
        public DOMTokenList classList { get; private set; }

        public Attr[] attributes { get; private set; }
        public string getAttribute(string name);
        public string? getAttributeNS(string? @namespace, string localName);
        public void setAttribute(string name, string value);
        public void setAttributeNS(string? @namespace, string name, string value);
        public void removeAttribute(string name);
        public void removeAttributeNS(string? @namespace, string localName);
        public bool hasAttribute(string name)
        {
            for (int i = 0; i < attributes.Length; i++)
            {
                if (attributes[i].name.NoncaseEqual(name))
                {
                    return true;
                }
            }
            return false;
        }
        public bool hasAttributeNS(string? @namespace, string localName);

        public HTMLCollection getElementsByTagName(string localName);
        public HTMLCollection getElementsByTagNameNS(string? @namespace, string localName);
        public HTMLCollection getElementsByClassName(string classNames);

        //DOM 4
        public HTMLCollection children { get; private set; }
        public Element? firstElementChild { get; private set; }
        public Element? lastElementChild { get; private set; }
        public Element? previousElementSibling { get; private set; }
        public Element? nextElementSibling { get; private set; }
        public long childElementCount { get; private set; }

        // NEW
        public void prepend(Node nodes);
        public void append(Node nodes);
        public void before(Node nodes);
        public void after(Node nodes);
        public void replace(Node nodes);
        public void prepend(string nodes);
        public void append(string nodes);
        public void before(string nodes);
        public void after(string nodes);
        public void replace(string nodes);
        public void remove();
        #endregion
    };
}
