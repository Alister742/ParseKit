using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements;
using ParseKit.DOMElements._Classes.Events;
using ParseKit.DOM.HTMLElements.Documents;

namespace ParseKit.DOM.DOMElements.Events
{
    // Event Constructor Syntax:
    //[Constructor(string typeArg, optional WheelEventInit wheelEventInitDict)]


    static class DeltaModeCode
    {
        public static long DOM_DELTA_PIXEL = 0x00;
        public static long DOM_DELTA_LINE = 0x01;
        public static long DOM_DELTA_PAGE = 0x02;

    }

    interface IWheelEvent /* : IMouseEvent */
    {
        // DeltaModeCode
        const long DOM_DELTA_PIXEL = 0x00;
        const long DOM_DELTA_LINE = 0x01;
        const long DOM_DELTA_PAGE = 0x02;

        double deltaX { get; }
        double deltaY { get; }
        double deltaZ { get; }
        long deltaMode { get; }


        // Originally introduced (and deprecated) in DOM Level 3:
        void initWheelEvent(string typeArg,
                            bool canBubbleArg,
                            bool cancelableArg,
                            AbstractView? viewArg,
                            long detailArg,
                            long screenXArg,
                            long screenYArg,
                            long clientXArg,
                            long clientYArg,
                             short buttonArg,
                            EventTarget? relatedTargetArg,
                            string modifiersListArg,
                            double deltaXArg,
                            double deltaYArg,
                            double deltaZArg,
                             long deltaMode);
    };
    // Suggested initWheelEvent replacement initializer:
    class WheelEventInit : MouseEventInit
    {
        // Attributes for WheelEvent:
        public double deltaX = 0.0;
        public double deltaY = 0.0;
        public double deltaZ = 0.0;
        public long deltaMode = 0;
    };
}

