using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.Core.Dom.Interfaces;

namespace Parse.DOM.DOMElements
{
    public class FilterResult
    {
        public const int FILTER_ACCEPT = 1;
        public const int FILTER_REJECT = 2;
        public const int FILTER_SKIP = 3;
    }

    // Constants for whatToShow
    enum whatToShow : long
    {
        SHOW_ALL = 0xFFFFFFFF,
        SHOW_ELEMENT = 0x1,
        SHOW_ATTRIBUTE = 0x2, // historical
        SHOW_TEXT = 0x4,
        SHOW_CDATA_SECTION = 0x8, // historical
        SHOW_ENTITY_REFERENCE = 0x10, // historical
        SHOW_ENTITY = 0x20, // historical
        SHOW_PROCESSING_INSTRUCTION = 0x40,
        SHOW_COMMENT = 0x80,
        SHOW_DOCUMENT = 0x100,
        SHOW_DOCUMENT_TYPE = 0x200,
        SHOW_DOCUMENT_FRAGMENT = 0x400,
        SHOW_NOTATION = 0x800, // historical
    }

    public class NodeFilter : INodeFilter
    {
        // Constants for whatToShow
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

        public int acceptNode(Node node)
        {
            //Let result be the return value of invoking filter.
            //dont know what it means... invoking method is acceptNode(Node node)??? DOM 3 have no his emplementation
            throw new NotImplementedException();
        }
    }
}
