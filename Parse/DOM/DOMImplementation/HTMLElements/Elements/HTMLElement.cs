//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using ParseKit.DOMElements._Classes.Nodes;

//namespace ParseKit.DOMSupport.HTMLElements.Elements
//{
//    interface HTMLElement : Element {
//  // metadata attributes
//           string title{get; set;}
//            string lang{get; set;}
//            bool translate{get; set;}
//            string dir{get; set;}
//    stringMap dataset{get; }

//  // microdata
//            bool itemScope{get; set;}
//  [PutForwards=value]DOMSettableTokenList itemType{get;}
//            string itemId{get; set;}
//  [PutForwards=value] DOMSettableTokenList itemRef{get; }
//  [PutForwards=value] DOMSettableTokenList itemProp{get;}
//        HTMLPropertiesCollection properties{get;}
//           object itemValue{get; set;}

//  // user interaction
//           bool hidden{get; set;}
//  void click();
//           long tabIndex{get; set;}
//          void focus();
//          void blur();
//           string accessKey{get; set;}
//            string accessKeyLabel{get; set;}
//           bool draggable{get; set;}
//  [PutForwards=value] DOMSettableTokenList dropzone{get;}
//           string contentEditable{get; set;}
//            bool isContentEditable{get; set;}
//           HTMLMenuElement contextMenu{get; set;}
//           bool spellcheck{get; set;}
//  void forceSpellCheck();

//  // command API
//  string commandType{get; }
//  string commandLabel{get; }
//  string commandIcon{get; }
//  bool commandHidden{get; }
//  bool commandDisabled{get;}
//  bool commandChecked{get;}

//  // styling
//  readonly attribute CSSStyleDeclaration style;

//  // event handler IDL attributes
//           attribute EventHandler onabort;
//           attribute EventHandler onblur;
//           attribute EventHandler oncancel;
//           attribute EventHandler oncanplay;
//           attribute EventHandler oncanplaythrough;
//           attribute EventHandler onchange;
//           attribute EventHandler onclick;
//           attribute EventHandler onclose;
//           attribute EventHandler oncontextmenu;
//           attribute EventHandler oncuechange;
//           attribute EventHandler ondblclick;
//           attribute EventHandler ondrag;
//           attribute EventHandler ondragend;
//           attribute EventHandler ondragenter;
//           attribute EventHandler ondragleave;
//           attribute EventHandler ondragover;
//           attribute EventHandler ondragstart;
//           attribute EventHandler ondrop;
//           attribute EventHandler ondurationchange;
//           attribute EventHandler onemptied;
//           attribute EventHandler onended;
//           attribute OnErrorEventHandler onerror;
//           attribute EventHandler onfocus;
//           attribute EventHandler oninput;
//           attribute EventHandler oninvalid;
//           attribute EventHandler onkeydown;
//           attribute EventHandler onkeypress;
//           attribute EventHandler onkeyup;
//           attribute EventHandler onload;
//           attribute EventHandler onloadeddata;
//           attribute EventHandler onloadedmetadata;
//           attribute EventHandler onloadstart;
//           attribute EventHandler onmousedown;
//           attribute EventHandler onmousemove;
//           attribute EventHandler onmouseout;
//           attribute EventHandler onmouseover;
//           attribute EventHandler onmouseup;
//           attribute EventHandler onmousewheel;
//           attribute EventHandler onpause;
//           attribute EventHandler onplay;
//           attribute EventHandler onplaying;
//           attribute EventHandler onprogress;
//           attribute EventHandler onratechange;
//           attribute EventHandler onreset;
//           attribute EventHandler onscroll;
//           attribute EventHandler onseeked;
//           attribute EventHandler onseeking;
//           attribute EventHandler onselect;
//           attribute EventHandler onshow;
//           attribute EventHandler onstalled;
//           attribute EventHandler onsubmit;
//           attribute EventHandler onsuspend;
//           attribute EventHandler ontimeupdate;
//           attribute EventHandler onvolumechange;
//           attribute EventHandler onwaiting;
//};

//interface HTMLUnknownElement : HTMLElement { };
//}
