using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Net;
using ParseKit.Proxy;
using ParseKit.Proxy.Parser;
using System.Collections;
using ParseKit.Data;
using ParseKit.Parsers;
using System.Threading;
using System.Security.Permissions;
using ParseKit.Data.Resourse.ResourceClasses;
using ParseKit.CORE;
using ParseKit.CapchaRecognize;
using ParseKit.Core.Dom;
using System.Xml;

namespace ParseKit
{
    class Program
    {
        static object sync = new object();

        void Main(string[] args)
        {
            Console.WriteLine("Start!");


            Console.Read();
            #region Proxy testing
            //GlobalLog.Erase();
            //ProxyManager.RemoveDuplicates();
            ////ProxyManager.RemoveRetardProxies();

            //ProxyTester prtest = new ProxyTester();
            //prtest.OnProxyTestComplete += new ProxyTester.ProxyTestHandler(pch_OnProxyTestComplete);
            //prtest.OnTestsComplete += new ProxyTester.CompleteHandler(pch_OnTestsComplete);
            //prtest.OnProgressChanged += new ProxyTester.ProgressHandler(prt_OnProgressChanged);
            //prtest.RegularTest(ProxyManager.LoadProxies());
            #endregion

            Console.WriteLine(DateTime.Now);
            Console.WriteLine("Main done!");
            Console.Read();
        }

        static void printChilds(XmlNodeList childs)
        {
            if (childs == null || childs.Count == 0)
                return;

            foreach (XmlNode child in childs)
            {
                Console.WriteLine(child.Name);

                XmlNodeList childsOfChild = child.ChildNodes;
                int lenght = childsOfChild.Count;

                printChilds(childsOfChild);
            }
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

        #region Void
        static void pg_OnParseCompleted()
        {
            Console.WriteLine("AND WORK DONE TOO");
        }

        static void pg_OnPageParsed(List<RatedProxy> proxies)
        {
            Console.WriteLine("{0} parsed", proxies.Count);
            ProxyManager.SaveProxies(proxies);
        }

        static void prt_OnProgressChanged(int left, bool unpingable)
        {
            //Console.Clear();
            Console.WriteLine("[{1}] {0} proxies left, unpingable-{2}", left, DateTime.Now.ToShortTimeString(), unpingable);
        }

        static void EndThs(DownloaderObj obj)
        {
            if (obj.Data != null)
            {
                Console.WriteLine("thread finally download {0} B", obj.Data.Length);
            }
            else
            {
                Console.WriteLine("WE GET RESPONSE WITH NULL DATA!!!");
            }
        }

        static void pch_OnTestsComplete()
        {
            Console.WriteLine("Downloads END {0}", DateTime.Now.ToShortTimeString());
            Console.WriteLine("Proxies Test Complete!!");
        }

        static void pch_OnProxyTestComplete(RatedProxy proxy)
        {
            //Console.WriteLine("{0} complete", proxy.Address.Host);
            List<RatedProxy> proxies = new List<RatedProxy>();
            proxies.Add(proxy);
            ProxyManager.SaveProxies(proxies);
        }

        private static void InitializeEvents()
        {
            //KeysGiver.OnKeyPageParsed += new EventHandler(KeysGiver_OnKeyPageParsed);
        }
        #endregion
    }
}
