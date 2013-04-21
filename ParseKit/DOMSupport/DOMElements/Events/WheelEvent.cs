using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements;
using ParseKit.DOM.HTMLElements.Documents;
using ParseKit.DOMElements._Classes.Events;

namespace ParseKit.DOM.DOMElements.Events
{
    // Event Constructor Syntax:
//[Constructor(DOMString typeArg, optional WheelEventInit wheelEventInitDict)]
class WheelEvent : MouseEvent, IWheelEvent
{
    public WheelEvent(string typeArg, WheelEventInit wheelEventInitDict = null) : base(typeArg, wheelEventInitDict)
    {
        if (wheelEventInitDict == null)
            wheelEventInitDict = new WheelEventInit();

            deltaX = wheelEventInitDict.deltaX;
            deltaY = wheelEventInitDict.deltaY;
            deltaZ = wheelEventInitDict.deltaZ;
            deltaMode = wheelEventInitDict.deltaMode;
    }

    #region Члены IWheelEvent


    public double deltaX { get; private set; }

    public double deltaY { get; private set; }

    public double deltaZ { get; private set; }

    public long deltaMode { get; private set; }

    public void initWheelEvent(string typeArg, bool canBubbleArg, bool cancelableArg, AbstractView? viewArg, long detailArg, long screenXArg, long screenYArg, long clientXArg, long clientYArg, short buttonArg, EventTarget? relatedTargetArg, string modifiersListArg, double deltaXArg, double deltaYArg, double deltaZArg, long deltaModeArg)
    {
        modifiersListArg = modifiersListArg.ToLower();
        bool ctrlKeyArg = modifiersListArg.Contains("control");
        bool altKeyArg = modifiersListArg.Contains("alt");
        bool shiftKeyArg = modifiersListArg.Contains("shift");
        bool metaKeyArg = modifiersListArg.Contains("meta");
        if (modifiersListArg.Contains("altgraph")) ctrlKeyArg = altKeyArg = true;

        base.initMouseEvent(typeArg, canBubbleArg, cancelableArg, viewArg, detailArg, screenXArg, screenYArg, clientXArg, clientYArg, ctrlKeyArg, altKeyArg, shiftKeyArg, metaKeyArg, buttonArg, relatedTargetArg);

        deltaX = deltaXArg;
        deltaY = deltaYArg;
        deltaZ = deltaZArg;
        deltaMode = deltaModeArg;
    }

    #endregion
};
}