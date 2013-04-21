using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.DOMElements._Classes.Nodes;
using ParseKit.DOMElements._Classes.Lists;
using ParseKit.DOM.HTMLElements.KnownElements.Scripting;

namespace ParseKit.DOMElements.HTMLClasses
{
class Document {
  // resource metadata management
  public   Location? location{get; private set;}
  public           string domain{get; set;}
 public    string referrer{get; private set;}
 public            string cookie{get; set;}
 public    string lastModified{get; private set;}
 public    string readyState{get; private set;}

  // DOM tree accessors
 public  getter object (string name);
   public          string title{get; set;}
   public          string dir{get; set;}
  public           HTMLElement? body{get; set;}
  public   HTMLHeadElement? head{get; private set;}
  public   HTMLCollection images{get; private set;}
  public   HTMLCollection embeds{get; private set;}
  public   HTMLCollection plugins{get; private set;}
  public   HTMLCollection links{get; private set;}
 public    HTMLCollection forms{get; private set;}
 public    HTMLCollection scripts{get; private set;}
public   NodeList getElementsByName(string elementName);
 public  NodeList getItems(optional string typeNames);
 public    DOMElementMap cssElementMap{get; private set;}
public     HTMLScriptElement? currentScript{get; private set;}

  // dynamic markup insertion
 public  Document open(optional string type, optional string replace);
public   WindowProxy open(string url, string name, string features, optional bool replace);
public   void close();
public   void write(string text);
public   void writeln(string text);

  // user interaction
  public   WindowProxy? defaultView{get; private set;}
  public   Element? activeElement{get; private set;}
 public  bool hasFocus();
        public     string designMode{get; set;}
 public  bool execCommand(string commandId);
 public  bool execCommand(string commandId, bool showUI);
 public  bool execCommand(string commandId, bool showUI, string value);
 public  bool queryCommandEnabled(string commandId);
 public  bool queryCommandIndeterm(string commandId);
 public  bool queryCommandState(string commandId);
  public bool queryCommandSupported(string commandId);
  public string queryCommandValue(string commandId);
  public HTMLCollection commands{get; private set;}

  // event handler IDL attributes
            public EventHandler onabort;
          public   EventHandler onblur;
          public   EventHandler oncancel;
          public   EventHandler oncanplay;
          public   EventHandler oncanplaythrough;
          public   EventHandler onchange;
           public  EventHandler onclick;
          public   EventHandler onclose;
          public   EventHandler oncontextmenu;
          public   EventHandler oncuechange;
          public   EventHandler ondblclick;
         public    EventHandler ondrag;
         public    EventHandler ondragend;
         public    EventHandler ondragenter;
           public  EventHandler ondragleave;
         public    EventHandler ondragover;
           public  EventHandler ondragstart;
           public  EventHandler ondrop;
            public EventHandler ondurationchange;
            public EventHandler onemptied;
            public EventHandler onended;
            public OnErrorEventHandler onerror;
            public EventHandler onfocus;
           public EventHandler oninput;
           public EventHandler oninvalid;
           public EventHandler onkeydown;
           public EventHandler onkeypress;
           public EventHandler onkeyup;
           public EventHandler onload;
           public EventHandler onloadeddata;
           public EventHandler onloadedmetadata;
           public EventHandler onloadstart;
           public EventHandler onmousedown;
           public EventHandler onmousemove;
           public EventHandler onmouseout;
           public EventHandler onmouseover;
           public EventHandler onmouseup;
           public EventHandler onmousewheel;
           public EventHandler onpause;
           public EventHandler onplay;
           public EventHandler onplaying;
           public EventHandler onprogress;
           public EventHandler onratechange;
           public EventHandler onreset;
           public EventHandler onscroll;
           public EventHandler onseeked;
           public EventHandler onseeking;
           public EventHandler onselect;
           public EventHandler onshow;
           public EventHandler onstalled;
           public EventHandler onsubmit;
           public EventHandler onsuspend;
           public EventHandler ontimeupdate;
           public EventHandler onvolumechange;
           public EventHandler onwaiting;

  // special event handler IDL attributes that only apply to Document objects
  [LenientThis] public EventHandler onreadystatechange;
};
}
