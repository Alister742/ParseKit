using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements;
using ParseKit.DOMElements._Classes.Events;
using ParseKit.DOM.HTMLElements.Documents;

namespace ParseKit.DOM.DOMElements.Events
{
    //[Constructor(string typeArg, optional FocusEventInit focusEventInitDict)]
    interface IFocusEvent /* : IUIEvent */
    {
        EventTarget? relatedTarget { get; }

        // Originally introduced (and deprecated) in DOM Level 3:
        void initFocusEvent(string typeArg, bool canBubbleArg, bool cancelableArg, AbstractView viewArg, long detailArg, EventTarget? relatedTargetArg);
    };

    // Suggested initFocusEvent replacement initializer:
    class FocusEventInit : UIEventInit
    {
        // Attributes for FocusEvent:
        public EventTarget relatedTarget = null;
    };
}
