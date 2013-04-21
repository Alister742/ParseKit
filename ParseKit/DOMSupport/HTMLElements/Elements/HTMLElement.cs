using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Nodes;

namespace ParseKit.DOMSupport.HTMLElements.Elements
{
    interface HTMLElement : Element {
  // metadata attributes
           attribute DOMString title;
           attribute DOMString lang;
           attribute boolean translate;
           attribute DOMString dir;
  readonly attribute DOMStringMap dataset;

  // microdata
           attribute boolean itemScope;
  [PutForwards=value] readonly attribute DOMSettableTokenList itemType;
           attribute DOMString itemId;
  [PutForwards=value] readonly attribute DOMSettableTokenList itemRef;
  [PutForwards=value] readonly attribute DOMSettableTokenList itemProp;
  readonly attribute HTMLPropertiesCollection properties;
           attribute any itemValue; // acts as DOMString on setting

  // user interaction
           attribute boolean hidden;
  void click();
           attribute long tabIndex;
  void focus();
  void blur();
           attribute DOMString accessKey;
  readonly attribute DOMString accessKeyLabel;
           attribute boolean draggable;
  [PutForwards=value] readonly attribute DOMSettableTokenList dropzone;
           attribute DOMString contentEditable;
  readonly attribute boolean isContentEditable;
           attribute HTMLMenuElement? contextMenu;
           attribute boolean spellcheck;
  void forceSpellCheck();

  // command API
  readonly attribute DOMString? commandType;
  readonly attribute DOMString? commandLabel;
  readonly attribute DOMString? commandIcon;
  readonly attribute boolean? commandHidden;
  readonly attribute boolean? commandDisabled;
  readonly attribute boolean? commandChecked;

  // styling
  readonly attribute CSSStyleDeclaration style;

  // event handler IDL attributes
           attribute EventHandler onabort;
           attribute EventHandler onblur;
           attribute EventHandler oncancel;
           attribute EventHandler oncanplay;
           attribute EventHandler oncanplaythrough;
           attribute EventHandler onchange;
           attribute EventHandler onclick;
           attribute EventHandler onclose;
           attribute EventHandler oncontextmenu;
           attribute EventHandler oncuechange;
           attribute EventHandler ondblclick;
           attribute EventHandler ondrag;
           attribute EventHandler ondragend;
           attribute EventHandler ondragenter;
           attribute EventHandler ondragleave;
           attribute EventHandler ondragover;
           attribute EventHandler ondragstart;
           attribute EventHandler ondrop;
           attribute EventHandler ondurationchange;
           attribute EventHandler onemptied;
           attribute EventHandler onended;
           attribute OnErrorEventHandler onerror;
           attribute EventHandler onfocus;
           attribute EventHandler oninput;
           attribute EventHandler oninvalid;
           attribute EventHandler onkeydown;
           attribute EventHandler onkeypress;
           attribute EventHandler onkeyup;
           attribute EventHandler onload;
           attribute EventHandler onloadeddata;
           attribute EventHandler onloadedmetadata;
           attribute EventHandler onloadstart;
           attribute EventHandler onmousedown;
           attribute EventHandler onmousemove;
           attribute EventHandler onmouseout;
           attribute EventHandler onmouseover;
           attribute EventHandler onmouseup;
           attribute EventHandler onmousewheel;
           attribute EventHandler onpause;
           attribute EventHandler onplay;
           attribute EventHandler onplaying;
           attribute EventHandler onprogress;
           attribute EventHandler onratechange;
           attribute EventHandler onreset;
           attribute EventHandler onscroll;
           attribute EventHandler onseeked;
           attribute EventHandler onseeking;
           attribute EventHandler onselect;
           attribute EventHandler onshow;
           attribute EventHandler onstalled;
           attribute EventHandler onsubmit;
           attribute EventHandler onsuspend;
           attribute EventHandler ontimeupdate;
           attribute EventHandler onvolumechange;
           attribute EventHandler onwaiting;
};

interface HTMLUnknownElement : HTMLElement { };
}
