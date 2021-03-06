﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.HTLMElements
{
    interface HTMLScriptElement : HTMLElement {
           attribute DOMString src;
           attribute DOMString type;
           attribute DOMString charset;
           attribute boolean async;
           attribute boolean defer;
           attribute DOMString crossOrigin;
           attribute DOMString text;
};
}
