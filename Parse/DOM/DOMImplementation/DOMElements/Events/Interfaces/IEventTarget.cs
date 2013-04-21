using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.Interfaces
{
    interface IEventTarget
    {
        void addEventListener(string type, EventListener callback, bool capture = false);
        void removeEventListener(string type, EventListener callback, bool capture = false);
        bool dispatchEvent(Event @event);
    };
}
