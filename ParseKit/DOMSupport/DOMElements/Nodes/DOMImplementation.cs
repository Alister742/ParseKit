using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParseKit.DOMElements._Classes.Nodes
{
    class DOMImplementation
    {
        public DocumentType createDocumentType(string qualifiedName, string publicId, string systemId);
        public XMLDocument createDocument(string? @namespace, string qualifiedName, DocumentType? doctype);
        public Document createHTMLDocument(string title = null);

        public bool hasFeature(string feature, string version);
    };
}
