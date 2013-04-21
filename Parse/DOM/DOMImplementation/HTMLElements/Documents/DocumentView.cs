using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.HTLMElements
{
    // Introduced in DOM Level 2:
    interface DocumentView
    {
        AbstractView defaultView { get; }
    };
}
