using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Parse.DOM.Interfaces
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
        double deltaX { get; }
        double deltaY { get; }
        double deltaZ { get; }
        long deltaMode { get; } // DeltaModeCode


        // Originally introduced (and deprecated) in DOM Level 3:
        void initWheelEvent(string typeArg,
                            bool canBubbleArg,
                            bool cancelableArg,
                            AbstractView viewArg,
                            long detailArg,
                            long screenXArg,
                            long screenYArg,
                            long clientXArg,
                            long clientYArg,
                            short buttonArg,
                            EventTarget relatedTargetArg,
                            string modifiersListArg,
                            double deltaXArg,
                            double deltaYArg,
                            double deltaZArg,
                            long deltaMode);
    }
    // Suggested initWheelEvent replacement initializer:
    class WheelEventInit : MouseEventInit
    {
        // Attributes for WheelEvent:
        public double deltaX = 0.0;
        public double deltaY = 0.0;
        public double deltaZ = 0.0;
        public long deltaMode = 0;
    }
}

