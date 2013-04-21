using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParseKit.DOMElements._Classes.Events
{
    class CustomEvent : Event, ICustomEvent
    {
        public CustomEvent(string type, CustomEventInit eveInit = null) : base(type, eveInit)
        {
            if (eveInit == null)
                eveInit = new CustomEventInit();

            detail = eveInit.detail;
        }

        public object detail { get; private set; }

        void initCustomEvent(string type, bool bubbles, bool cancelable, object details)
        {
            base.initEvent(type, bubbles, cancelable);
            detail = details;
        }
    };
}
