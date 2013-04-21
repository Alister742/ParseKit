using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.Interfaces
{
    // Event Constructor Syntax:
    //[Constructor(DOMString typeArg, optional MouseEventInit mouseEventInitDict)]
    interface IMouseEvent /* : IUIEvent */
    {
        long screenX { get; }
        long screenY { get; }
        long clientX { get; }
        long clientY { get; }
        bool ctrlKey { get; }
        bool shiftKey { get; }
        bool altKey { get; }
        bool metaKey { get; }
        short button { get; }
        int buttons{ get; }
        EventTarget relatedTarget { get; }
        void initMouseEvent(string typeArg,
                                           bool canBubbleArg,
                                           bool cancelableArg,
                                           AbstractView viewArg,
                                           long detailArg,
                                           long screenXArg,
                                           long screenYArg,
                                           long clientXArg,
                                           long clientYArg,
                                           bool ctrlKeyArg,
                                           bool altKeyArg,
                                           bool shiftKeyArg,
                                           bool metaKeyArg,
                                           short buttonArg,
                                           EventTarget relatedTargetArg);
        // Introduced in DOM Level 3:
        bool getModifierState(string keyArg);
    }

    // Suggested initMouseEvent replacement initializer:
    class MouseEventInit : UIEventInit
    {
        // Attributes for MouseEvent:
        public long screenX = 0;
        public long screenY = 0;
        public long clientX = 0;
        public long clientY = 0;
        public bool ctrlKey = false;
        public bool shiftKey = false;
        public bool altKey = false;
        public bool metaKey = false;
        public short button = 0;
        // Note: "buttons" was not previously initializable through initMouseEvent!
        public short buttons = 0;
        public EventTarget relatedTarget = null;
    };
}
