using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements;
using ParseKit.DOM.HTMLElements.Documents;
using System.Windows.Forms;

namespace ParseKit.DOM.DOMElements.Events
{
    //[Constructor(DOMString typeArg, optional KeyboardEventInit keyboardEventInitDict)]
class KeyboardEvent : UIEvent, IKeyboardEvent
{
    public KeyboardEvent(string typeArg, KeyboardEventInit keyboardEventInitDict = null) : base(typeArg, keyboardEventInitDict)
    {
        if (keyboardEventInitDict == null)
            keyboardEventInitDict = new KeyboardEventInit();

        @char = keyboardEventInitDict.@char;
        key = keyboardEventInitDict.key;
        location = keyboardEventInitDict.location;
        ctrlKey = keyboardEventInitDict.ctrlKey;
        shiftKey = keyboardEventInitDict.shiftKey;
        altKey = keyboardEventInitDict.altKey;
        metaKey = keyboardEventInitDict.metaKey;
        repeat = keyboardEventInitDict.repeat;
        locale = keyboardEventInitDict.locale;
        charCode = keyboardEventInitDict.charCode;
        keyCode = keyboardEventInitDict.keyCode;
        which = keyboardEventInitDict.which;
    }

    void DecodeModifiersString(string modifiersListArg)
    {
        modifiersListArg = modifiersListArg.ToLower();

        ctrlKey = modifiersListArg.Contains("control");
        altKey = modifiersListArg.Contains("alt");
        shiftKey = modifiersListArg.Contains("shift");
        metaKey = modifiersListArg.Contains("meta");

        if (modifiersListArg.Contains("altgraph")) ctrlKey = altKey = true;
    }

    #region Члены IKeyboardEvent

    public string @char { get; private set; }

    /// <summary>
    /// val equal to Keys.[key].ToString()
    /// </summary>
    public string key { get; private set; }

    public long location { get; private set; }

    public bool ctrlKey { get; private set; }

    public bool shiftKey { get; private set; }

    public bool altKey { get; private set; }

    public bool metaKey { get; private set; }

    public bool repeat { get; private set; }

    public string locale { get; private set; }

    public bool getModifierState(string keyArg)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// charCode holds a character value, for keypress events which generate character input.
    /// </summary>
    public long charCode { get; private set; }

    /// <summary>
    /// keyCode holds a system- and implementation-dependent numerical code signifying the unmodified identifier associated with the key pressed.
    /// </summary>
    public long keyCode { get; private set; }

    /// <summary>
    /// which holds a system- and implementation-dependent numerical code signifying the unmodified identifier associated with the key pressed.
    /// </summary>
    public long which { get; private set; }

    public void initKeyboardEvent(string typeArg, bool canBubbleArg, bool cancelableArg, AbstractView? viewArg, string charArg, string keyArg, long locationArg, string modifiersListArg, bool repeatArg, string localeArg)
    {
        base.initUIEvent(typeArg, canBubbleArg, cancelableArg, viewArg, 0);

        DecodeModifiersString(modifiersListArg);
        @char = charArg;
        key = keyArg;
        location = locationArg;
        repeat = repeatArg;
        locale = localeArg;
        charCode = (long)@char[0];
        try { keyCode = (int)new KeysConverter().ConvertFromString(keyArg); }
        catch (Exception) { }
        which = keyCode;
    }

    #endregion
};
}
