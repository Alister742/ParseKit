using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Jint.Native;
using ParseKit.Core.Dom.Interfaces;

namespace Parse.DOM.DOMElements
{
    //class not implementing DOM realization, need to refactor methods after check DOM model
    public class EventTarget : IEventTarget
    {
        Dictionary<string, TargetInfo> _events = new Dictionary<string,TargetInfo>();

        public class TargetInfo
        {
            public object callback;
            public bool capture;
        }

        #region Члены IEventTarget

        public void addEventListener(string type, object callback, bool capture = false)
        {
            //if (!(callback is IJSMethod || callback is Delegate))
            //    throw new DOMException((int)ExceptionCodes.VALIDATION_ERR);

            var targetInfo = new TargetInfo() { callback = callback, capture = capture };

            if (!_events.ContainsKey(type))
            {
                _events.Add(type, targetInfo);
            }
            else
            {
                _events[type] = targetInfo;
            }
        }

        public void removeEventListener(string type, object callback, bool capture = false)
        {
            if (_events.ContainsKey(type))
            {
                _events.Remove(type);
            }
        }

        public bool dispatchEvent(Event evt, object[] args = null)
        {
            TargetInfo target;
            if (_events.TryGetValue(evt.type, out target))
            {
                //if (target.callback is IJSMethod)
                //{
                //    throw new NotImplementedException();
                //    (target.callback as IJSMethod).Invoke(args, null);
                //}
                //else if (target.callback is Delegate)
                //{
                //    throw new NotImplementedException();
                //    (target.callback as Delegate).DynamicInvoke(args);
                //}
                return evt.defaultPrevented ? false : true;
            }
            else
            {
                throw new DOMException((int)ExceptionCodes.INVALID_STATE_ERR);
            }
        }

        #endregion

        #region Члены IEventTarget

        public void addEventListener(string type, EventListener callback, bool capture = false)
        {
            throw new NotImplementedException();
        }

        public void removeEventListener(string type, EventListener callback, bool capture = false)
        {
            throw new NotImplementedException();
        }

        public bool dispatchEvent(Event @event)
        {
            throw new NotImplementedException();
        }

        #endregion
    };
}
