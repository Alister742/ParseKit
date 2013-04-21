using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Events;
using ParseKit.DOMElements;
using ParseKit.DOMElements._Classes.Errors;
using ParseKit.DOM.DOMElements.Events;

namespace ParseKit.DOMSupport.DOMElements.Events
{
    static class EventFactory
    {
        public static IEvent CreateEventByType(string type)
        {
            if (type.NoncaseEqual("UIEvent")) return new UIEvent(string.Empty);

            if (type.NoncaseEqual("MouseEvent")) return new MouseEvent(string.Empty);

            if (type.NoncaseEqual("MutationEvent")) return new MutationEvent();

            if (type.NoncaseEqual("WheelEvent")) return new WheelEvent(string.Empty);

            if (type.NoncaseEqual("KeyboardEvent")) return new KeyboardEvent(string.Empty);

            if (type.NoncaseEqual("CustomEvent")) return new CustomEvent(string.Empty);

            if (type.NoncaseEqual("FocusEvent")) return new FocusEvent(string.Empty);

            if (type.NoncaseEqual("CompositionEvent")) return new CompositionEvent(string.Empty);

            //Instead of: throw new DOMException() { code = ExceptionCodes.NOT_SUPPORTED_ERR };
            return new Event(string.Empty); 
        }
    }


}
