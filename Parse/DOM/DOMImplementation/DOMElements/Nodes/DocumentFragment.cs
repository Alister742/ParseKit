using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.Core.Dom.Interfaces;

namespace Parse.DOM.DOMElements
{
    public class DocumentFragment : Node, IDocumentFragment
    {
        public DocumentFragment(Document doc) : base(doc)
        {

        }
        //NEW
        public void prepend(Node nodes)
        {
            throw new NotImplementedException();
        }
        public void append(Node nodes)
        {
            throw new NotImplementedException();
        }
        public void prepend(string nodes)
        {
            throw new NotImplementedException();
        }
        public void append(string nodes)
        {
            throw new NotImplementedException();
        }
    };
}
