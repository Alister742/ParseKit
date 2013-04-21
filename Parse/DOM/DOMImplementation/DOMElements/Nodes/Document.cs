using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.Core.Dom.Interfaces;
using ParseKit.Core.Dom.DOMElements.Nodes;
using System.IO;

namespace Parse.DOM.DOMElements
{
    public delegate void XmlNodeChangedEventHandler(object sender, NodeChangedEventArgs args);

    public class Document : Node, IDocument, IDocumentEvent
    {
        public ITreeBuilder TreeBuilder;
        public Tokenizer tokenizer;
        public readonly JavascriptContext JS;
        public readonly bool EnableScripting;

        private XmlNodeChangedEventHandler onNodeInsertingDelegate;
        private XmlNodeChangedEventHandler onNodeInsertedDelegate;
        private XmlNodeChangedEventHandler onNodeRemovingDelegate;
        private XmlNodeChangedEventHandler onNodeRemovedDelegate;
        private XmlNodeChangedEventHandler onNodeChangingDelegate;
        private XmlNodeChangedEventHandler onNodeChangedDelegate;

        public Encoding Encoding { get; private set; }

        public Document(Tokenizer tokenizer = null, ITreeBuilder tb = null,  bool enableScripting = true, Encoding encoding = null, string contentType = "application/xml", string url = "about:blank") 
            : base(null)
        {
            EnableScripting = enableScripting;

            Encoding = encoding ?? Encoding.UTF8;
            this.contentType = contentType;
            URL = url;

            documentElement = new Element("document", this);
            documentElement.parentNode = this;

            /* dom tree constructors classes */

            TreeBuilder = tb;
            //TreeBuilder = new DomTreeBuilder(this);
            this.tokenizer = tokenizer;

            //INIT HERE THE START VARIABLES, like: document, window
            JS = new JavascriptContext();
        }

        public void Load(TextReader reader)
        {
            tokenizer.Start();
            bool swallowBom = true;

            try
            {
                char[] buffer = new char[2048];
                UTF16Buffer bufr = new UTF16Buffer(buffer, 0, 0);
                bool lastWasCR = false;
                int len = -1;
                if ((len = reader.Read(buffer, 0, buffer.Length)) != 0)
                {
                    int streamOffset = 0;
                    int offset = 0;
                    int length = len;
                    if (swallowBom)
                    {
                        if (buffer[0] == '\uFEFF')
                        {
                            streamOffset = -1;
                            offset = 1;
                            length--;
                        }
                    }
                    if (length > 0)
                    {
                        bufr.Start = offset;
                        bufr.End = offset + length;
                        while (bufr.HasMore)
                        {
                            bufr.Adjust(lastWasCR);
                            lastWasCR = false;
                            if (bufr.HasMore)
                            {
                                lastWasCR = tokenizer.TokenizeBuffer(bufr);
                            }
                        }
                    }
                    streamOffset = length;
                    while ((len = reader.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        bufr.Start = 0;
                        bufr.End = len;
                        while (bufr.HasMore)
                        {
                            bufr.Adjust(lastWasCR);
                            lastWasCR = false;
                            if (bufr.HasMore)
                            {
                                lastWasCR = tokenizer.TokenizeBuffer(bufr);
                            }
                        }
                        streamOffset += len;
                    }
                }
                tokenizer.Eof();
            }
            finally
            {
                tokenizer.End();
            }
        }
        public object ExecScript(string script)
        {
            return JS.Run(script);
        }

        internal void BeforeEvent(NodeChangedEventArgs args)
        {
            if (args != null)
            {
                switch (args.Action)
                {
                    case NodeChangedAction.Insert:
                        if (onNodeInsertingDelegate != null)
                            onNodeInsertingDelegate(this, args);
                        break;

                    case NodeChangedAction.Remove:
                        if (onNodeRemovingDelegate != null)
                            onNodeRemovingDelegate(this, args);
                        break;

                    case NodeChangedAction.Change:
                        if (onNodeChangingDelegate != null)
                            onNodeChangingDelegate(this, args);
                        break;
                }
            }
        }
        internal void AfterEvent(NodeChangedEventArgs args)
        {
            if (args != null)
            {
                switch (args.Action)
                {
                    case NodeChangedAction.Insert:
                        if (onNodeInsertedDelegate != null)
                            onNodeInsertedDelegate(this, args);
                        break;

                    case NodeChangedAction.Remove:
                        if (onNodeRemovedDelegate != null)
                            onNodeRemovedDelegate(this, args);
                        break;

                    case NodeChangedAction.Change:
                        if (onNodeChangedDelegate != null)
                            onNodeChangedDelegate(this, args);
                        break;
                }
            }
        }

        //TODO: delete this?
        private bool reportValidity;

        internal NodeChangedEventArgs GetEventArgs(Node node, Node oldParent, Node newParent, string oldValue, string newValue, NodeChangedAction action)
        {
            reportValidity = false;

            switch (action)
            {
                case NodeChangedAction.Insert:
                    if (onNodeInsertingDelegate == null && onNodeInsertedDelegate == null)
                    {
                        return null;
                    }
                    break;
                case NodeChangedAction.Remove:
                    if (onNodeRemovingDelegate == null && onNodeRemovedDelegate == null)
                    {
                        return null;
                    }
                    break;
                case NodeChangedAction.Change:
                    if (onNodeChangingDelegate == null && onNodeChangedDelegate == null)
                    {
                        return null;
                    }
                    break;
            }
            return new NodeChangedEventArgs(node, oldParent, newParent, oldValue, newValue, action);
        }

        #region IDocument
        public DOMImplementation implementation { get; private set; }
        public string URL { get; private set; }
        public string documentURI { get { return URL; } }
        public string compatMode { get; private set; }
        public string characterSet { get; private set; }
        public string contentType { get; private set; }

        public DocumentType doctype { get; private set; }
        public Element documentElement { get; private set; }
        public HTMLCollection getElementsByTagName(string localName)
        {
            throw new NotImplementedException();
        }
        public HTMLCollection getElementsByTagNameNS(string @namespace, string localName)
        {
            throw new NotImplementedException();
        }
        public HTMLCollection getElementsByClassName(string classNames)
        {
            throw new NotImplementedException();
        }
        public Element getElementById(string elementId)
        {
            throw new NotImplementedException();
        }

        public Element createElement(string localName)
        {
            if (localName.ToLower().Contains("http"))
            {
                
            }
            Element e = new Element(localName.ToLower(), this, "http://www.w3.org/1999/xhtml");

            return e;
        }
        public Element createElementNS(string nspace, string qualifiedName)
        {
            if (qualifiedName.ToLower().Contains("http"))
            {

            }
            nspace = nspace == string.Empty ? null : nspace;

            string prefix = null;
            if (qualifiedName.Contains(':'))
            {
                string[] temp = qualifiedName.Split(new char[] { ':' }, 2);
                prefix = temp[0];
            }

            Element e = new Element(qualifiedName.ToLower(), this, nspace, prefix);

            return e;
        }
        public DocumentFragment createDocumentFragment()
        {
            return new DocumentFragment(this);
        }
        public Text createTextNode(string data)
        {
            return new Text(data, this);
        }
        public Comment createComment(string data)
        {
            return new Comment(data, this);
        }
        public ProcessingInstruction createProcessingInstruction(string target, string data)
        {
            return new ProcessingInstruction(target, this, data);
        }

        public Node importNode(Node node, bool deep = true)
        {
            if (node.nodeType == (int)NodeType.DOCUMENT_NODE)
            {
                throw new DOMError("The operation is not supported.");
            }

            Node n = node.cloneNode(deep);
            n.ownerDocument = this;

            return n;
        }
        public Node adoptNode(Node node)
        {
            throw new NotImplementedException();
        }

        public Event createEvent(string eventType)
        {
            Event e = EventFactory.CreateEventByType(eventType) as Event;
            if (e==null)
	        {
		        throw new DOMError("The operation is not supported.");
	        }

            return e; 
        }

        public Range createRange()
        {
            throw new NotImplementedException();

            //return new Range();
        }

        public NodeIterator createNodeIterator(Node root, long whatToShow = NodeFilter.SHOW_ALL, NodeFilter filter = null)
        {
            return new NodeIterator(root, whatToShow, filter);
        }
        public TreeWalker createTreeWalker(Node root, long whatToShow = NodeFilter.SHOW_ALL, NodeFilter filter = null)
        {
            return new TreeWalker(root, whatToShow, filter);
        }

        public void prepend(Node nodes)
        {
            throw new NotImplementedException();
        }

        public void append(Node nodes)
        {
            throw new NotImplementedException();
        }

        public void prepend(string nodes)
        {
            throw new NotImplementedException();
        }

        public void append(string nodes)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region JavaScript Methods

        public void write(string data)
        {
            char[] buffer = data.ToCharArray();
            UTF16Buffer bufr = new UTF16Buffer(buffer, 0, 0);
            tokenizer.TokenizeBuffer(bufr);
        }
        public void writeln(string data)
        {
            write(data + '\u000a');
        }

        #endregion
    };

    public class XMLDocument : Document 
    { 
    };
}