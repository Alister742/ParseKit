using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParseKit.DOMElements._Classes.Events
{
    /*
     * 
     *  var e = document.createEvent("Event");
        e.initEvent("myevent", false, false);
        var target = document.createElement("div");
        target.addEventListener("myevent", function () {
           throw "Error";
        });
        target.dispatchEvent(e);
     * 
     * 
     *  var chartData = ...;
        var evt = document.createEvent("CustomEvent");
        evt.initCustomEvent( "updateChart", true, false, { data: chartData });
        document.documentElement.dispatchEvent(evt);
     */

    class Event : IEvent
    {
        public Event(string type, EventInit init = null)
        {
            this.type = type;
            if (init == null)
                init = new EventInit();

            bubbles = init.bubbles;
            cancelable = init.cancelable;
            
            this.isTrusted = false;
            this.eventPhase = EventPhase.NONE;
            this.target = null;
            this.timeStamp = (long)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
        }

        #region Члены IEvent

        public string type { get; private set; }

        public IEventTarget? target { get; private set; }

        public IEventTarget? currentTarget
        {
            get { throw new NotImplementedException(); }
        }

        //EvenPhase enum
        public short eventPhase { get; private set; }

        public void stopPropagation()
        {
            /*
             * Prevents all other event listeners from being triggered, excluding any remaining candiate event listeners. Once it has been called, further calls to this method have no additional effect.
             */
        }

        public void stopImmediatePropagation()
        {
            /*
            Prevents all other event listeners from being triggered for this event dispatch, including any remaining candiate event listeners. Once it has been called, further calls to this method have no additional effect.
            */
        }

        public bool bubbles { get; private set; }

        public bool cancelable { get; private set; }

        public void preventDefault()
        {
            if (cancelable)
            {
                /*
                 * When this method is invoked, the event must be canceled, meaning any default actions normally taken by the implementation as a result of the event must not occur (see also Default actions and cancelable events). Default actions which occur prior to the event's dispatch (see Default actions and cancelable events) are reverted. Calling this method for a non-cancelable event must have no effect. If an event has more than one default action, each cancelable  default action must be canceled.
                 */
            }
        }

        public bool defaultPrevented { get; private set; }

        public bool isTrusted { get; private set; }

        public /*TimeSpan*/ long timeStamp { get; private set; }

        public void initEvent(string type, bool bubbles, bool cancelable)
        {
            this.type = type ?? "";
            this.bubbles = bubbles;
            this.cancelable = cancelable;

            this.defaultPrevented = false;
        }

        #endregion
    }
}
