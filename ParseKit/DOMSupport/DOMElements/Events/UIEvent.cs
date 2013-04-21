using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Events;
using ParseKit.DOM.HTMLElements.Documents;

namespace ParseKit.DOMElements
{
    // Introduced in DOM Level 2:
    //[Constructor(DOMString typeArg, optional UIEventInit dictUIEventInit)]
    class UIEvent : Event, IUIEvent
    {
        public UIEvent(string typeArg, UIEventInit dictUIEventInit = null)
            : base(typeArg, dictUIEventInit)
        {
            if (dictUIEventInit == null)
                dictUIEventInit = new UIEventInit();
                view = dictUIEventInit.view;
                detail = dictUIEventInit.detail;

        }

        #region Члены IUIEvent

        public AbstractView? view { get; private set; }

        public long detail { get; private set; }

        public void initUIEvent(string type, bool canBubble, bool cancelable, AbstractView? viewArg, long detailArg)
        {
            base.initEvent(type, canBubble, cancelable);
            view = viewArg;
            detail = detailArg;
        }

        #endregion
    };
}
