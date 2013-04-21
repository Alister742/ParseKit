using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.Interfaces
{
    interface IDOMImplementation
    {
         DocumentType createDocumentType(string qualifiedName, string Id, string systemId);
         XMLDocument createDocument(string @namespace, string qualifiedName, DocumentType doctype);
         Document createHTMLDocument(string title = null);

         bool hasFeature(string feature, string version);
    }
}
