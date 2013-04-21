using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParseKit.DOMElements._Classes.Errors
{
    class DOMException : System.Exception
    {
        public /*ExceptionCodes*/ short code;
    }
}
