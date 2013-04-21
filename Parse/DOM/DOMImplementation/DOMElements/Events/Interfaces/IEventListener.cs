using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.Interfaces
{
// Introduced in DOM Level 2:
    interface IEventListener
    {
        void handleEvent(Event evt);
    }
}
