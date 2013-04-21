using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.Core.Dom.Interfaces;
using System.Threading;


namespace Parse.DOM
{
    public class DomTreeBuilder : TreeBuilder<Element>
    {
        public delegate void AppendDel(Element element, string text);
        public delegate void CreateDel(string ns, string name, HtmlAttributes attributes);
        public delegate void AppendElementDel(Element child, Element newParent);
        public delegate void AppendCommentToDocumentDel(string comment);
        public delegate void AppendDoctypeToDocumentDel(string name, string publicIdentifier, string systemIdentifier);
        public event CreateDel OnCreateElement;
        public event AppendDel OnAppendComment;
        public event AppendElementDel OnAppendElement;
        public event AppendDel OnAppendCharacters;
        public event AppendCommentToDocumentDel OnAppendCommentToDocument;
        public event AppendDoctypeToDocumentDel OnAppendDoctypeToDocument;

        AutoResetEvent waiter = new AutoResetEvent(true);

        private Document document;
        public Document Document
        {
            get { return document; }
        }

        public bool IsDebug { get; set; }

        public void Continue()
        {
            waiter.Set();
            GlobalLog.Write("Debug continue", LogChannel.NOTIFY_MSG);
        }


        public DomTreeBuilder(Document doc)
        {
            this.document = doc;
        }

        private void AppendCommentToDocument(string comment)
        {
            if (OnAppendCommentToDocument != null)
            {
                OnAppendCommentToDocument(comment);
            }

            if (IsDebug)
            {
                GlobalLog.Write("Stop in AppendCommentToDocument", LogChannel.NOTIFY_MSG);
                waiter.WaitOne();
            }

            document.appendChild(document.createComment(comment));
        }

        private void AppendComment(Element parent, string comment)
        {
            if (OnAppendComment != null)
            {
                OnAppendComment(parent, comment);
            }

            if (IsDebug)
            {
                GlobalLog.Write("Stop in AppendComment", LogChannel.NOTIFY_MSG);
                waiter.WaitOne();
            }

            parent.appendChild(document.createComment(comment));
        }

        protected override Element CreateElement(string ns, string name, HtmlAttributes attributes)
        {
            if (OnCreateElement!=null)
            {
                OnCreateElement(ns, name, attributes);
            }

            if (IsDebug)
            {
                GlobalLog.Write("Stop in CreateElement", LogChannel.NOTIFY_MSG);
                waiter.WaitOne();
            }

            Element rv = document.createElementNS(ns, name);
            for (int i = 0; i < attributes.Length; i++)
            {
                rv.setAttributeNS(attributes.GetURI(i), attributes.GetLocalName(i), attributes.GetValue(i));
            }
            return rv;
        }

        protected override void InsertFosterParentedCharacters(char[] buf, int start, int length, Element table, Element stackParent)
        {
            string text = new String(buf, start, length);

            Node parent = table.parentNode;
            if (parent != null)
            { // always an element if not null
                Node previousSibling = table.previousSibling;
                if (previousSibling != null
                        && previousSibling.nodeType == (int)NodeType.TEXT_NODE)
                {
                    Text lastAsText = (Text)previousSibling;
                    lastAsText.data += text;
                    return;
                }
                parent.insertBefore(document.createTextNode(text), table);
                return;
            }
            Node lastChild = stackParent.lastChild;
            if (lastChild != null && lastChild.nodeType == (int)NodeType.TEXT_NODE)
            {
                Text lastAsText = (Text)lastChild;
                lastAsText.data += text;
                return;
            }
            stackParent.appendChild(document.createTextNode(text));

        }

        protected override void AppendCharacters(Element parent, char[] buf, int start, int length)
        {
            AppendCharacters(parent, new String(buf, start, length));
        }

        protected override void AppendCommentToDocument(char[] buf, int start, int length)
        {
            AppendCommentToDocument(new String(buf, start, length));
        }

        protected override void AppendIsindexPrompt(Element parent)
        {
            AppendCharacters(parent, "This is a searchable index. Enter search keywords: ");
        }

        protected override void AppendComment(Element parent, char[] buf, int start, int length)
        {
            AppendComment(parent, new String(buf, start, length));
        }

        protected override void InsertFosterParentedChild(Element child, Element table, Element stackParent)
        {
            Node parent = table.parentNode;
            if (parent != null)
            { // always an element if not null
                parent.insertBefore(child, table);
            }
            else
            {
                stackParent.appendChild(child);
            }
        }

        protected override void AppendChildrenToNewParent(Element oldParent, Element newParent)
        {
            while (oldParent.hasChildNodes())
            {
                newParent.appendChild(oldParent.firstChild);
            }
        }

        protected override void AppendElement(Element child, Element newParent)
        {
            if (OnAppendElement != null)
            {
                OnAppendElement(child, newParent);
            }

            if (IsDebug)
            {
                GlobalLog.Write("Stop in OnAppendElement", LogChannel.NOTIFY_MSG);
                waiter.WaitOne();
            }

            newParent.appendChild(child);
            if (child.tagName == "script" && document.EnableScripting)
            {
                document.ExecScript(null);
            }
        }

        protected override bool HasChildren(Element element)
        {
            return element.hasChildNodes();
        }

        protected override void DetachFromParent(Element element)
        {
            Node parent = element.parentNode;
            if (parent != null)
            {
                parent.removeChild(element);
            }
        }

        protected override Element CreateHtmlElementSetAsRoot(HtmlAttributes attributes)
        {
            Element rv = document.createElementNS("http://www.w3.org/1999/xhtml", "html");
            for (int i = 0; i < attributes.Length; i++)
            {
                rv.setAttributeNS(attributes.GetURI(i), attributes.GetLocalName(i), attributes.GetValue(i));
            }

            document.appendChild(rv);
            return rv;
        }

        private void AppendCharacters(Element parent, string text)
        {
            if (OnAppendCharacters != null)
            {
                OnAppendCharacters(parent, text);
            }

            if (IsDebug)
            {
                GlobalLog.Write("Stop in AppendCharacters", LogChannel.NOTIFY_MSG);
                waiter.WaitOne();
            }

            Node lastChild = parent.lastChild;
            if (lastChild != null && lastChild.nodeType == (int)NodeType.TEXT_NODE)
            {
                Text lastAsText = (Text)lastChild;
                lastAsText.data += text;
                return;
            }
            parent.appendChild(document.createTextNode(text));
        }

        protected override void AppendDoctypeToDocument(string name, string publicIdentifier, string systemIdentifier)
        {
            if (OnAppendDoctypeToDocument != null)
            {
                OnAppendDoctypeToDocument(name, publicIdentifier, systemIdentifier);
            }

            if (IsDebug)
            {
                GlobalLog.Write("Stop in AppendDoctypeToDocument", LogChannel.NOTIFY_MSG);
                waiter.WaitOne();
            }
            

            if (publicIdentifier == String.Empty)
                publicIdentifier = null;
            if (systemIdentifier == String.Empty)
                systemIdentifier = null;

            var doctype = new DocumentType(name, document, publicIdentifier, systemIdentifier);
            document.appendChild(doctype);
        }

        protected override void MarkMalformedIfScript(Element elt)
        {

        }

        protected override void Start(bool fragmentMode)
        {
        }

        protected override void AddAttributesToElement(Element element, HtmlAttributes attributes)
        {
            for (int i = 0; i < attributes.Length; i++)
            {
                String localName = attributes.GetLocalName(i);
                String uri = attributes.GetURI(i);
                if (!element.hasAttributeNS(uri, localName))
                {
                    element.setAttributeNS(uri, localName, attributes.GetValue(i));
                }
            }
        }
    }
}
