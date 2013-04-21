using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParseKit.DOMElements._Classes.Events
{
    interface IEventTarget
    {
        void addEventListener(string type, IEventListener? callback, bool capture = false);
        void removeEventListener(string type, IEventListener? callback, bool capture = false);
        bool dispatchEvent(IEvent @event);
    };
}
