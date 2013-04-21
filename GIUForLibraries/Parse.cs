using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using ParseKit.Core.Dom;
using ParseKit;
using System.Xml;

namespace GIUForLibraries
{
    public partial class Parse : Form
    {
        public Parse()
        {
            InitializeComponent();
        }

        static void Tokenize(TextReader reader, Tokenizer tokenizer)
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

        public void LoadXml(TextReader reader)
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

        Tokenizer tokenizer;


        private void button1_Click(object sender, EventArgs e)
        {
            GlobalLog.Write("Parse start...");

            using (StreamReader sr = new StreamReader(textBox1.Text, Encoding.UTF8))
            {
                XmlDocument doc = new XmlDocument();


                DomTreeBuilderOrig TreeBuilder = new DomTreeBuilderOrig(doc);
                tokenizer = new Tokenizer(TreeBuilder);

                //doc.TreeBuilder = TreeBuilder;
                //doc.tokenizer = tokenizer;


                //tokenizer.OnError += new Tokenizer.NotifyDel(tokenizer_OnError);
                //tokenizer.OnFatal += new Tokenizer.NotifyDel(tokenizer_OnFatal);
                //tokenizer.OnWarn += new Tokenizer.NotifyDel(tokenizer_OnWarn);

                //TreeBuilder.OnEndTag += new TreeBuilder<XmlElement>.EndTagDel(TreeBuilder_OnEndTag);
                //TreeBuilder.OnError += new TreeBuilder<XmlElement>.NotifyDel(TreeBuilder_OnError);
                //TreeBuilder.OnFatal += new TreeBuilder<XmlElement>.NotifyDel(TreeBuilder_OnFatal);
                //TreeBuilder.OnStartTag += new TreeBuilder<XmlElement>.StartTagDel(TreeBuilder_OnStartTag);
                //TreeBuilder.OnTokenData += new TreeBuilder<XmlElement>.TokenDataDel(TreeBuilder_OnTokenData);
                //TreeBuilder.OnWarring += new TreeBuilder<XmlElement>.NotifyDel(TreeBuilder_OnWarring);

                TreeBuilder.OnAppendDoctypeToDocument += new DomTreeBuilderOrig.AppendDoctypeToDocumentDel(TreeBuilder_AppendDoctypeToDocument);
                TreeBuilder.OnAppendCharacters += new DomTreeBuilderOrig.AppendDel(TreeBuilder_OnAppendCharacters);
                TreeBuilder.OnAppendComment += new DomTreeBuilderOrig.AppendDel(TreeBuilder_OnAppendComment);
                TreeBuilder.OnAppendCommentToDocument += new DomTreeBuilderOrig.AppendCommentToDocumentDel(TreeBuilder_OnAppendCommentToDocument);
                TreeBuilder.OnAppendElement += new DomTreeBuilderOrig.AppendElementDel(TreeBuilder_OnAppendElement);
                TreeBuilder.OnCreateElement += new DomTreeBuilderOrig.CreateDel(TreeBuilder_OnCreateElement);

                //LoadXml(sr);

                Tokenize(sr, tokenizer);

                //doc.Load(sr);

                //Document dosssc = TreeBuilder.Document;
                printChilds(TreeBuilder.Document.ChildNodes);
            }
        }

        void TreeBuilder_OnEnd(object sender, EventArgs e)
        {
            GlobalLog.Write("TreeBuilder finished", "TreeBulder");
        }

        void TreeBuilder_OnCreateElement(string ns, string name, HtmlAttributes attributes)
        {
            GlobalLog.Write(string.Format("CreateElement {0} with {1} attributes", name, attributes.Length), "TreeBulder");
        }

        void TreeBuilder_OnAppendElement(XmlElement child, XmlElement newParent)
        {
            GlobalLog.Write(string.Format("AppendElement {0} to ", child.LocalName, newParent.LocalName), "TreeBulder");
        }

        void TreeBuilder_OnAppendCommentToDocument(string comment)
        {
            GlobalLog.Write(string.Format("AppendCommentToDocument {0}", comment), "TreeBulder");
        }

        void TreeBuilder_OnAppendComment(XmlElement element, string text)
        {
            GlobalLog.Write(string.Format("AppendComment {0}, to {1}", text, element.LocalName), "TreeBulder");
        }

        void TreeBuilder_OnAppendCharacters(XmlElement element, string text)
        {
            GlobalLog.Write(string.Format("AppendComment {0}, to {1}", text, element.LocalName), "TreeBulder");
        }

        void TreeBuilder_AppendDoctypeToDocument(string name, string publicIdentifier, string systemIdentifier)
        {
            GlobalLog.Write(string.Format("AppendDoctypeToDocument {0}", name), "TreeBulder");
        }

        void TreeBuilder_OnWarring(string msg)
        {
            GlobalLog.Write(string.Format("TreeBuilder warn: {0}", msg), "TreeBulder");
        }

        void TreeBuilder_OnTokenData(string text)
        {
            GlobalLog.Write(string.Format("Token data: {0}", text), "TreeBulder");
        }

        void TreeBuilder_OnStartTag(ElementName eltName, HtmlAttributes attributes, bool selfClosing)
        {
            GlobalLog.Write(string.Format("Start tag {0} with {1} attributes, selfClosing <{2}>", eltName.name, attributes.Length, selfClosing), "TreeBulder");
        }

        void TreeBuilder_OnFatal(string msg)
        {
            GlobalLog.Write(string.Format("TreeBuilder fatal: {0}", msg), "TreeBulder");
        }

        void TreeBuilder_OnError(string msg)
        {
            GlobalLog.Write(string.Format("TreeBuilder error: {0}", msg), "TreeBulder");
        }

        void TreeBuilder_OnEndTag(ElementName eltName)
        {
            GlobalLog.Write(string.Format("End tag: {0}", eltName.name), "TreeBulder");
        }

        void tokenizer_OnWarn(string msg)
        {
            GlobalLog.Write(string.Format("Tokenizer warn: {0}", msg), "Tokenizer");
        }

        void tokenizer_OnFatal(string msg)
        {
            GlobalLog.Write(string.Format("Tokenizer fatal: {0}", msg), "Tokenizer");
        }

        void tokenizer_OnError(string msg)
        {
            GlobalLog.Write(string.Format("Tokenizer error: {0}", msg), "Tokenizer");
        }




        static void printChilds(XmlNodeList childs)
        {
            if (childs == null || childs.Count == 0)
                return;

            foreach (XmlNode child in childs)
            {
                GlobalLog.Write(child.Name, "DOM_parsekit");

                XmlNodeList childsOfChild = child.ChildNodes;
                int lenght = childsOfChild.Count;

                printChilds(childsOfChild);
            }
        }
    }
}
