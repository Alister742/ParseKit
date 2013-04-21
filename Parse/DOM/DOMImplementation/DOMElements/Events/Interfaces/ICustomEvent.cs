using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.Interfaces
{
    interface ICustomEvent //: IEvent
    {
        //public abstract ICustomEvent(string type) : base(type)

        object detail { get;}

        void initCustomEvent(string type, bool bubbles, bool cancelable, object details);
    };

    class CustomEventInit : EventInit
    {
        public object detail;
    };
}
