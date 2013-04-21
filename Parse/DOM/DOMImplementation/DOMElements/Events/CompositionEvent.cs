using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ParseKit.Core.Dom.Interfaces;

namespace Parse.DOM.DOMElements
{
    //[Constructor(DOMString typeArg, optional CompositionEventInit compositionEventInitDict)]
class CompositionEvent : UIEvent, ICompositionEvent
{
    public CompositionEvent(string typeArg, CompositionEventInit compositionEventInitDict =null) : base(typeArg, compositionEventInitDict)
    {
        if (compositionEventInitDict == null)
            compositionEventInitDict = new CompositionEventInit();

        data = compositionEventInitDict.data;
        locale = compositionEventInitDict.locale;
    }

    #region Члены ICompositionEvent

    public string data { get; private set; }

    public string locale { get; private set; }

    public void initCompositionEvent(string typeArg, bool canBubbleArg, bool cancelableArg, AbstractView viewArg, string dataArg, string localeArg)
    {
        base.initUIEvent(typeArg, canBubbleArg, cancelableArg, viewArg, 0);

        data = dataArg;
        locale = localeArg;
    }

    #endregion
};
}
