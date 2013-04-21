using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.Core.Dom.Interfaces;

namespace Parse.DOM.DOMElements
{
    static class EventFactory
    {
        public static IEvent CreateEventByType(string type)
        {
            type = type.ToLower();
            switch (type)
            {
                case "event":
                case "events":
                case "htmlevents":
                    return new Event(type);
                case "mouseevent":
                case "mouseevents":
                    return new MouseEvent(type);
                case "uievent":
                case "uievents":
                    return new UIEvent(type);
                default:
                    return new Event();
            }


            //if (type.NoncaseEqual("UIEvent")) return new UIEvent(string.Empty);

            //if (type.NoncaseEqual("MouseEvent")) return new MouseEvent(string.Empty);

            //if (type.NoncaseEqual("MutationEvent")) return new MutationEvent();

            //if (type.NoncaseEqual("WheelEvent")) return new WheelEvent(string.Empty);

            //if (type.NoncaseEqual("KeyboardEvent")) return new KeyboardEvent(string.Empty);

            //if (type.NoncaseEqual("CustomEvent")) return new CustomEvent(string.Empty);

            //if (type.NoncaseEqual("FocusEvent")) return new FocusEvent(string.Empty);

            //if (type.NoncaseEqual("CompositionEvent")) return new CompositionEvent(string.Empty);

            ////Instead of: throw new DOMException() { code = ExceptionCodes.NOT_SUPPORTED_ERR };
            //return new Event(string.Empty); 
        }
    }


}
