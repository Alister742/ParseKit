using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Events;

namespace ParseKit.DOMElements
{
// Introduced in DOM Level 2:
    class EventListener : IEventListener
    {
        #region Члены IEventListener

        /// <summary>
        /// This method must be called whenever an event occurs of the event type for which the EventListener interface was registered.
        /// </summary>
        /// <param name="evt"></param>
        public void handleEvent(IEvent evt)
        {
            //evt.
        }

        #endregion
    }
}
