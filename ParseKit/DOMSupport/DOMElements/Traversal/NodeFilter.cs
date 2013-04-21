using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Nodes;

namespace ParseKit.DOMElements._Classes.Traversal
{
    class NodeFilter
    {
        // public constants for acceptNode()
        public const short FILTER_ACCEPT = 1;
        public const short FILTER_REJECT = 2;
        public const short FILTER_SKIP = 3;

        // public constants for whatToShow
        public const long SHOW_ALL = 0xFFFFFFFF;
        public const long SHOW_ELEMENT = 0x1;
        public const long SHOW_ATTRIBUTE = 0x2; // historical
        public const long SHOW_TEXT = 0x4;
        public const long SHOW_CDATA_SECTION = 0x8; // historical
        public const long SHOW_ENTITY_REFERENCE = 0x10; // historical
        public const long SHOW_ENTITY = 0x20; // historical
        public const long SHOW_PROCESSING_INSTRUCTION = 0x40;
        public const long SHOW_COMMENT = 0x80;
        public const long SHOW_DOCUMENT = 0x100;
        public const long SHOW_DOCUMENT_TYPE = 0x200;
        public const long SHOW_DOCUMENT_FRAGMENT = 0x400;
        public const long SHOW_NOTATION = 0x800; // historical

        public short acceptNode(Node node);
    };
}
