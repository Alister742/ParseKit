using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParseKit.DOMElements._Classes.Nodes
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

    class DocumentType : Node
    {
        public string name { get; private set; }
        public string publicId { get; private set; }
        public string systemId { get; private set; }

        // NEW
        void before(Node nodes);
        void after(Node nodes);
        void replace(Node nodes);
        void before(string nodes);
        void after(string nodes);
        void replace(string nodes);
        void remove();
    }
}
