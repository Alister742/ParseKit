using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements;
using ParseKit.DOM.HTMLElements.Documents;

namespace ParseKit.DOM.DOMElements.Events
{

    //[Constructor(string typeArg, optional CompositionEventInit compositionEventInitDict)]
    interface ICompositionEvent /* : IUIEvent */
    {
        string? data { get; }
        string locale { get; }


        // Originally introduced (and deprecated) in DOM Level 3:
        void initCompositionEvent(string typeArg,
                               bool canBubbleArg,
                               bool cancelableArg,
                               AbstractView? viewArg,
                               string? dataArg,
                               string localeArg);
    };

    // Suggested initCompositionEvent replacement initializer:
    class CompositionEventInit : UIEventInit
    {
        // Attributes for CompositionEvent:
        public string? data = null;
        public string locale = "";
    };
}
