using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Events;
using ParseKit.DOM.HTMLElements.Documents;

namespace ParseKit.DOMElements
{
    // troduced  DOM Level 2:
    
// Event Constructor Syntax:
//[Constructor(DOMString typeArg, optional MouseEventInit mouseEventInitDict)]
    class MouseEvent : UIEvent, IMouseEvent
    {
        public MouseEvent(string typeArg, MouseEventInit mouseEventInitDict = null)
            : base(typeArg, mouseEventInitDict)
        {
            if (mouseEventInitDict == null)
                mouseEventInitDict = new MouseEventInit();

            screenX = mouseEventInitDict.screenX;
            screenY = mouseEventInitDict.screenY;
            clientX = mouseEventInitDict.clientX;
            clientY = mouseEventInitDict.clientY;
            ctrlKey = mouseEventInitDict.ctrlKey;
            shiftKey = mouseEventInitDict.shiftKey;
            altKey = mouseEventInitDict.altKey;
            metaKey = mouseEventInitDict.metaKey;
            button = mouseEventInitDict.button;
            relatedTarget = mouseEventInitDict.relatedTarget;
        }

        #region Члены IMouseEvent

        public long screenX { get; private set; }

        public long screenY { get; private set; }

        public long clientX { get; private set; }

        public long clientY { get; private set; }

        public bool ctrlKey { get; private set; }

        public bool shiftKey { get; private set; }

        public bool altKey { get; private set; }

        public bool metaKey { get; private set; }

        /// <summary>
        /// 0 must indicate the primary button of the device (in general, the left button or the only button on single-button devices, used to activate a user interface control or select text) or the un-initialized value.
        /// 1 must indicate the auxiliary button (in general, the middle button, often combined with a mouse wheel).
        /// 2 must indicate the secondary button (in general, the right button, often used to display a context menu).
        /// </summary>
        public short button { get; private set; }

        /// <summary>
        /// 0 must indicates no button is currently active.
        ///1 must indicate the primary button of the device (in general, the left button or the only button on single-button devices, used to activate a user interface control or select text).
        ///2 must indicate the secondary button (in general, the right button, often used to display a context menu), if present.
        ///4 must indicate the auxiliary button (in general, the middle button, often combined with a mouse wheel).
        /// </summary>
        public int buttons
        {
            get
            {
                if (button == 0) return 1;
                if (button == 1) return 4;
                if (button == 2) return 2;
                return 0;
            }
        }

        public EventTarget? relatedTarget { get; private set; }

        public void initMouseEvent(string typeArg, bool canBubbleArg, bool cancelableArg, AbstractView? viewArg, long detailArg, long screenXArg, long screenYArg, long clientXArg, long clientYArg, bool ctrlKeyArg, bool altKeyArg, bool shiftKeyArg, bool metaKeyArg, short buttonArg, EventTarget? relatedTargetArg)
        {
            base.initUIEvent(typeArg, canBubbleArg, cancelableArg, viewArg, detailArg);

            screenX = screenXArg;
            screenY = screenYArg;
            clientX = clientXArg;
            clientY = clientYArg;
            ctrlKey = ctrlKeyArg;
            shiftKey = shiftKeyArg;
            altKey = altKeyArg;
            metaKey = metaKeyArg;
            button = buttonArg;
            relatedTarget = relatedTargetArg;
        }

        public bool getModifierState(string keyArg)
        {
            return altKey || shiftKey || ctrlKey || metaKey;
        }

        #endregion
    }
}
/*
5.2.3.2 Mouse Event Order

Certain mouse events defined in this specification occur in a set order relative to one another. The following is the typical sequence of events when a pointing device's cursor is moved over an element:
mousemove (Pointing device is moved into an element...)
mouseover
mouseenter
mousemove (multiple events) (Pointing device is moved out of the element...)
mouseout
mouseleave

When a pointing device is moved into an element A, and then into a nested element B and then back out again, the following is the typical sequence of events:
mousemove (Pointing device is moved into element A...)
mouseover (element A)
mouseenter (element A)
mousemove (multiple events in element A) (Pointing device is moved into nested element B...)
mouseout (element A)
mouseover (element B)
mouseenter (element B)mousemove (multiple events in element B) (Pointing device is moved from element B into A...)
mouseout (element B)
mouseleave (element B)
mouseover (element A)mousemove (multiple events in element A) (Pointing device is moved out of element A...)
mouseout (element A)
mouseleave (element A)

When the pointing device is moved from outside the element stack to the element labeled C and then moved out again, the following series of events occur:
mousemove(Pointing device is moved into element C; the topmost element in the stack)
mouseover (element C)
mouseenter (element A)
mouseenter (element B)
mouseenter (element C)
mousemove (multiple events in element C)(Pointing device is moved out of element C...)
mouseout (element C)
mouseleave (element C)
mouseleave (element B)
mouseleave (element A)
 * 
 * 
 * 
 * 
 * The following is the typical sequence of events when a button associated with a pointing device (e.g., a mouse button or trackpad) is pressed and released over an element:
mousedown
mousemove (optional, multiple events, some limits)
mouseup
click
mousemove (optional, multiple events, some limits)
mousedown
mousemove (optional, multiple events, some limits)
mouseup
click
dblclick
*/