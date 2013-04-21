using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.Core.Dom.Interfaces;

namespace Parse.DOM.DOMElements
{
    public class DOMImplementation : IDOMImplementation
    {
        #region Члены IDOMImplementation

        public DocumentType createDocumentType(string qualifiedName, string Id, string systemId)
        {
            throw new NotImplementedException();
        }

        public XMLDocument createDocument(string @namespace, string qualifiedName, DocumentType doctype)
        {
            throw new NotImplementedException();
        }

        public Document createHTMLDocument(string title = null)
        {
            throw new NotImplementedException();
        }

        public bool hasFeature(string feature, string version)
        {
            throw new NotImplementedException();
        }

        #endregion
    };
}
