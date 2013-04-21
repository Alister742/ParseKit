using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParseKit.DOMSupport.AJAX
{
    class XMLHttpRequestResponseType
    {

        string _typeStr;
        private XMLHttpRequestResponseType(string type)
        {
            _typeStr = type;
        }

        public static XMLHttpRequestResponseType Arraybuffer
        {
            get { return new XMLHttpRequestResponseType("arraybuffer"); }
        }
        public static XMLHttpRequestResponseType Blob
        {
            get { return new XMLHttpRequestResponseType("blob"); }
        }
        public static XMLHttpRequestResponseType Document
        {
            get { return new XMLHttpRequestResponseType("document"); }
        }
        public static XMLHttpRequestResponseType Json
        {
            get { return new XMLHttpRequestResponseType("json"); }
        }
        public static XMLHttpRequestResponseType Text
        {
            get { return new XMLHttpRequestResponseType("text"); }
        }
        public static XMLHttpRequestResponseType Empty
        {
            get { return new XMLHttpRequestResponseType(""); }
        }

        public override string ToString()
        {
            return _typeStr;
        }
    };
}
