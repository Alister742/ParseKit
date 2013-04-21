using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Events;

namespace Parse.DOM.AJAX
{
    class XMLHttpRequestEventTarget : EventTarget
    {
        // event handlers
        public EventHandler onloadstart;
        public EventHandler onprogress;
        public EventHandler onabort;
        public EventHandler onerror;
        public EventHandler onload;
        public EventHandler ontimeout;
        public EventHandler onloadend;
    };
}
