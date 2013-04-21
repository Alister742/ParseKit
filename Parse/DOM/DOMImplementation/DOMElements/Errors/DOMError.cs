using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.Core.Dom.Interfaces;

namespace Parse.DOM.DOMElements
{
    class DOMError : Exception, IDOMError
    {
        public DOMError()
            : base()
        {
        }

        public DOMError(string mgs)
            : base(mgs)
        {
            this.name = mgs;
        }

        #region Члены IDOMError

        public string name
        {
            get;
            private set;
        }

        #endregion
    }
}
