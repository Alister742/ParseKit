using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Parse.DOM.DOMElements
{
    public class EventInfo
    {
        public EventInfo(Type[] trustedTargetTypes)
        {
            for (int i = 0; i < trustedTargetTypes.Length; i++)
            {
                TrustedTargetTypes.Add(trustedTargetTypes[i]);
            }
        }

        public string Name;
        public bool Async;
        public bool BubblingPhase;
        public bool Cancelable;

        public List<Type> TrustedTargetTypes = new List<Type>();
        public string DomInterface;

        public Action DefaultAction;
    }

    public class KnownEventsList
    {
        static Dictionary<string, EventInfo> _knownEvents = new Dictionary<string, EventInfo>();
            
        static KnownEventsList()
        {
            //new EventInfo(new Type[] {typeof(Element)}) { Async = false, Cancelable = false, BubblingPhase = false, DomInterface = "", Name = "", TrustedTargetTypes = null };
            //_knownEvents.Add("abort", new EventInfo("abort", "Event", new Type[] { typeof(Element) }));
            //_knownEvents.Add("blur", new EventInfo("blur", "FocusEvent", new Type[] { typeof(Element) }));
            //_knownEvents.Add("abort", new EventInfo("click", "MouseEvent", new Type[] { typeof(Element) }, /*DOMActivate event*/ null, true, false, true));
            //_knownEvents.Add("abort", new EventInfo("compositionstart", "CompositionEvent", new Type[] { typeof(Element) }));
            //_knownEvents.Add("abort", new EventInfo("abort", "Event", new Type[] { typeof(Element) }));

        }

        public EventInfo this[string name]
        {
            get
            {
                if (name == null)
                    return null;

                EventInfo eve;
                _knownEvents.TryGetValue(name, out eve);
                return eve;
            }
        }
    }
}
