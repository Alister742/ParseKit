using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParseKit.DOMElements._Classes.Events
{
    abstract class ICustomEvent : IEvent
    {
        //public abstract ICustomEvent(string type) : base(type)

        public object detail { get; private set; }

        void initCustomEvent(string type, bool bubbles, bool cancelable, object details);
    };

    class CustomEventInit : EventInit
    {
        public object detail;
    };
}
