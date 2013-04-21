using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parse.DOM.Interfaces
{
    //[Constructor(string typeArg, optional UIEventInit dictUIEventInit)]
    interface IUIEvent /* : Event */
    {
        AbstractView view { get; }
        long detail { get; }
        void initUIEvent(string typeArg, bool canBubbleArg, bool cancelableArg, AbstractView viewArg, long detailArg);
    };

    class UIEventInit : EventInit
    {
        public AbstractView view = null;
        public long detail = 0;
    }
}
