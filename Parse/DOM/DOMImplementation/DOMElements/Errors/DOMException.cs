using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.Core.Dom.Interfaces;

namespace Parse.DOM.DOMElements
{
    class DOMException : Exception, IDOMException
    {
        public DOMException()
            : base()
        {
        }

        public DOMException(ExceptionCodes code)
            : base((code).ToString())
        {
            this.code = (int)code;
        }

        public DOMException(int code)
            : base(((ExceptionCodes)code).ToString())
        {
            this.code = code;
        }

        /*ExceptionCodes*/
        public int code { get; private set; }
    }
}
