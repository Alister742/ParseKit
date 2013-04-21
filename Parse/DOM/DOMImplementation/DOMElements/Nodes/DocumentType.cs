using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.DOMElements
{
    public enum DocType : byte
    {
        /// <summary>
        /// Use the default doc type (from CsQuery.Config.DocType).
        /// </summary>
        Default = 0,
        /// <summary>
        /// HTML5
        /// </summary>
        HTML5 = 1,
        /// <summary>
        /// HTML 4 Transitional
        /// </summary>
        HTML4 = 2,
        /// <summary>
        /// XHTML -- all tags will be explicitly closed.
        /// </summary>
        XHTML = 3,
        /// <summary>
        /// An unsupported document type.
        /// </summary>
        Unknown = 4,
        /// <summary>
        /// HTML 4 Strict
        /// </summary>
        HTML4Strict = 5

    }

    public class DocumentType : NodeDom4
    {
        public DocumentType(string name, Document doc, string publicId = "", string systemId = "")
            : base(doc)
        {
            this.name = name;
            this.publicId = publicId;
            this.systemId = systemId;
        }

        public string name { get; private set; }
        public string publicId { get; private set; }
        public string systemId { get; private set; }
    }
}
