using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements;
using ParseKit.DOM.HTMLElements.Documents;
using System.Windows.Forms;

namespace ParseKit.DOM.DOMElements.Events
{
    /// <summary>
    /// This set of constants must be used to indicate the location of a key on the device. 
    /// In case a DOM implementation wishes to provide a new location information, 
    /// a value different from the following constant values must be used.
    /// </summary>
    static class KeyLocationCode
    {
        public static int GetKeyLocationCode(Keys key)
        {
            switch (key)
            {
                case Keys.RShiftKey:
                case Keys.RMenu:
                case Keys.RControlKey:
                case Keys.RWin:
                    return DOM_KEY_LOCATION_RIGHT;

                case Keys.LShiftKey:
                case Keys.LMenu:
                case Keys.LControlKey:
                case Keys.LWin:
                    return DOM_KEY_LOCATION_LEFT;

                case Keys.NumPad0:
                case Keys.NumPad1:
                case Keys.NumPad2:
                case Keys.NumPad3:
                case Keys.NumPad4:
                case Keys.NumPad5:
                case Keys.NumPad6:
                case Keys.NumPad7:
                case Keys.NumPad8:
                case Keys.NumPad9:
                    return DOM_KEY_LOCATION_NUMPAD;

                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                    return DOM_KEY_LOCATION_JOYSTICK;

                default:
                    return DOM_KEY_LOCATION_STANDARD;
            }
        }

        public static int DOM_KEY_LOCATION_STANDARD = 0x00;
        public static int DOM_KEY_LOCATION_LEFT = 0x01;
        public static int DOM_KEY_LOCATION_RIGHT = 0x02;
        public static int DOM_KEY_LOCATION_NUMPAD = 0x03;
        public static int DOM_KEY_LOCATION_MOBILE = 0x04; /* UNSUPPORTED */
        public static int DOM_KEY_LOCATION_JOYSTICK = 0x05;
    }

    //[Constructor(string typeArg, optional KeyboardEventInit keyboardEventInitDict)]
    interface IKeyboardEvent /* : IUIEvent */
    {
        // KeyLocationCode
        long DOM_KEY_LOCATION_STANDARD = 0x00;
        long DOM_KEY_LOCATION_LEFT = 0x01;
        long DOM_KEY_LOCATION_RIGHT = 0x02;
        long DOM_KEY_LOCATION_NUMPAD = 0x03;
        long DOM_KEY_LOCATION_MOBILE = 0x04;
        long DOM_KEY_LOCATION_JOYSTICK = 0x05;

        string @char { get; }
        string key { get; }
        // KeyLocationCode
        long location { get; }
        bool ctrlKey { get; }
        bool shiftKey { get; }
        bool altKey { get; }
        bool metaKey { get; }
        bool repeat { get; }
        string locale { get; }
        bool getModifierState(string keyArg);

        // The following support legacy user agents:
        long charCode { get; }
        long keyCode { get; }
        long which { get; }


        // Originally introduced (and deprecated) in DOM Level 3:
        void initKeyboardEvent(string typeArg,
                               bool canBubbleArg,
                               bool cancelableArg,
                               AbstractView? viewArg,
                               string charArg,
                               string keyArg,
                                long locationArg,
                               string modifiersListArg,
                               bool repeat,
                               string localeArg);
    };


    // Suggested initKeyboardEvent replacement initializer:
    class KeyboardEventInit : UIEventInit
    {
        // Attributes for KeyboardEvent:
        public string @char = "";
        public string key = "";
        public long location = 0; //KeyLocationCode
        public bool ctrlKey = false;
        public bool shiftKey = false;
        public bool altKey = false;
        public bool metaKey = false;
        public bool repeat = false;
        public string locale = "";

        // (Legacy) key attributes for KeyboardEvent:
        public long charCode = 0;
        public long keyCode = 0;
        public long which = 0;
    };
}
