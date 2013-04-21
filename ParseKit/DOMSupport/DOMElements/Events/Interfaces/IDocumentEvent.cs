using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Events;

namespace ParseKit.DOMElements
{
// Introduced in DOM Level 2:
    interface IDocumentEvent
    {
        IEvent createEvent(string eventType);
        //raises(DOMException);
    }

}
