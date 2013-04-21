using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements;
using ParseKit.DOMElements._Classes.Events;
using ParseKit.DOM.HTMLElements.Documents;

namespace ParseKit.DOM.DOMElements.Events
{
    //[Constructor(DOMString typeArg, optional FocusEventInit focusEventInitDict)]
class FocusEvent : UIEvent, IFocusEvent
{
    public FocusEvent(string typeArg, FocusEventInit focusEventInitDict = null) : base(typeArg, focusEventInitDict)
    {
        if (focusEventInitDict == null)
            focusEventInitDict = new FocusEventInit();

        relatedTarget = focusEventInitDict.relatedTarget;
    }

    #region Члены IFocusEvent

    public EventTarget? relatedTarget { get; private set; }

    public void initFocusEvent(string typeArg, bool canBubbleArg, bool cancelableArg, AbstractView viewArg, long detailArg, EventTarget? relatedTargetArg)
    {
        base.initUIEvent(typeArg, canBubbleArg, cancelableArg, viewArg, detailArg);
        relatedTarget = relatedTargetArg;
    }

    #endregion
};
}
