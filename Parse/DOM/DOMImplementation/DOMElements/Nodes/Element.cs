using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.Core.Dom.Interfaces;

namespace Parse.DOM.DOMElements
{
    public class Element : NodeDom4, IElement
    {
        string name;

        #region Constructotrs

        public Element(string localName, List<Attr> attributes, Document doc)
            : this(localName, doc)
        {
            this.attributes = attributes;
        }

        public Element(string localName, Document doc, string namespaceURI = null, string prefix = null)
            : base(doc)
        {
            if (attributes == null)
            {
                attributes = new List<Attr>();
            }

            this.localName = localName;
            this.namespaceURI = namespaceURI;
            this.prefix = prefix;

            //if (this.parentNode == null)
            //{
                
            //}
        }
        #endregion

        public bool TryGetAttribyte(string name, out Attr attr)
        {
            for (int i = 0; i < attributes.Count; i++)
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
        public string namespaceURI { get; private set; }
        public string prefix { get; private set; }
        public string localName { get; private set; }
        public string tagName
        {
            get
            {
                if (name == null)
                {
                    if (prefix != null && prefix.Length > 0)
                    {
                        if (localName.Length > 0)
                        {
                            name = string.Concat(prefix, ":", localName);
                        }
                        else
                        {
                            name = prefix;
                        }
                    }
                    else
                    {
                        name = localName;
                    }
                }
                return name;
            }
        }

        public string id
        {
            get
            {
                return getAttribute("id") ?? string.Empty;
            }
            set
            {
                setAttribute("id", value);
            }
        }
        public string className
        {
            get
            {
                return getAttribute("class") ?? string.Empty;
            }
            set
            {
                setAttribute("class", value);
            }
        }
        public DOMTokenList classList { get; private set; }

        public List<Attr> attributes { get; private set; }
        public string getAttribute(string name)
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                if (attributes[i].name.NoncaseEqual(name))
                {
                    return attributes[i].value;
                }
            }
            return null;
        }
        public string getAttributeNS(string nspace, string localName)
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                if (attributes[i].localName.NoncaseEqual(localName) &&
                    attributes[i].namespaceURI.NoncaseEqual(nspace))
                {
                    return attributes[i].value;
                }
            }
            return null;
        }
        public void setAttribute(string name, string value)
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                if (attributes[i].name.NoncaseEqual(name))
                {
                    attributes[i].value = value;
                    return;
                }
            }
            attributes.Add(new Attr(name, value));
        }
        public void setAttributeNS(string nspace, string name, string value)
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                if (attributes[i].name.NoncaseEqual(name) &&
                    attributes[i].namespaceURI.NoncaseEqual(nspace))
                {
                    attributes[i].value = value;
                    return;
                }
            }
            attributes.Add(new Attr(name, value, null, nspace));
        }
        public void removeAttribute(string name)
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                if (attributes[i].name.NoncaseEqual(name))
                {
                    attributes.RemoveAt(i);
                    return;
                }
            }
        }
        public void removeAttributeNS(string nspace, string localName)
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                if (attributes[i].localName.NoncaseEqual(localName) &&
                    attributes[i].namespaceURI.NoncaseEqual(nspace))
                {
                    attributes.RemoveAt(i);
                    return;
                }
            }
        }
        public bool hasAttribute(string name)
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                if (attributes[i].name.NoncaseEqual(name))
                {
                    return true;
                }
            }
            return false;
        }
        public bool hasAttributeNS(string nspace, string localName)
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                if (attributes[i].localName.NoncaseEqual(localName) &&
                    attributes[i].namespaceURI.NoncaseEqual(nspace))
                {
                    return true;
                }
            }
            return false;
        }

        public HTMLCollection getElementsByTagName(string localName)
        {
            HTMLCollection elments = new HTMLCollection();

            foreach (Node child in childNodes)
            {
                if (child.nodeName.NoncaseEqual(localName))
                {
                    elments.Add(child as Element);
                }
                elments.AddRange((child as Element).getElementsByTagName(localName));
            }
            return elments;
        }

        public HTMLCollection getElementsByTagNameNS(string nspace, string localName)
        {
            HTMLCollection elments = new HTMLCollection();

            foreach (Node child in childNodes)
            {
                if (child.nodeName.NoncaseEqual(localName) &&
                    (child as Element).namespaceURI.NoncaseEqual(nspace))
                {
                    elments.Add(child as Element);
                }
                elments.AddRange((child as Element).getElementsByTagNameNS(nspace, localName));
            }
            return elments;
        }
        public HTMLCollection getElementsByClassName(string classNames)
        {
            HTMLCollection elments = new HTMLCollection();

            foreach (var child in childNodes)
            {
                if ((child as Element).className.NoncaseEqual(classNames))
                {
                    elments.Add(child as Element);
                }
                elments.AddRange((child as Element).getElementsByClassName(classNames));
            }
            return elments;
        }

        //DOM 4
        public HTMLCollection children { get { return new HTMLCollection(childNodes); } }
        public Element firstElementChild { get { return this.children.First(); } }
        public Element lastElementChild { get { return this.children.Last(); } }
        public Element previousElementSibling
        {
            get
            {
                int idx = parentElement.childNodes.IndexOf(this);
                if (idx > 0)
                {
                    return parentElement.childNodes[idx - 1] as Element;
                }
                else
                    return null;
            }
        }
        public Element nextElementSibling
        {
            get
            {
                int idx = parentElement.childNodes.IndexOf(this);
                if (idx < parentElement.childNodes.length - 1)
                {
                    return parentElement.childNodes[idx + 1] as Element;
                }
                else
                    return null;
            }
        }
        public long childElementCount { get { return children.Count; } }
        #endregion
    };
}
