using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.Core.Dom.Interfaces;

namespace Parse.DOM.DOMElements
{
    public class Attr : IAttr
    {
        public Attr(string name, string value = null, string prefix = null, string namespaceURI = null)
        {
            this.name = name;
            this.value = value;
            this.namespaceURI = namespaceURI;
            this.prefix = prefix;
        }

        public string localName { get; private set; }
        public string value { get; set; }

        public string name { get; private set; }
        public string namespaceURI { get; private set; }
        public string prefix { get; private set; }
    }
}
