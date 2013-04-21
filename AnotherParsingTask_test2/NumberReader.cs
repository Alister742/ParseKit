using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ParseKit.Data;
using HtmlAgilityPack;
using System.Threading;
using ParseKit.Interfaces;

namespace AnotherParsingTask_test2
{
    /// <summary>
    /// This is empty emplementation of IPageReader interface, only for estimated example
    /// </summary>
    public class NumberReader : IPageReader
    {
        public event NewTargetsDel OnNewTargets;
        public event ReadCompleteDel OnReadComplete;

        public void ReadData(string data, DevourTarget target)
        {
            List<DevourTarget> targets = new List<DevourTarget>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(data);

            //HtmlNodeCollection pageCountArea = doc.DocumentNode.SelectNodes("//h3/span[@class='rot']");
            int count = 5; // (Int32.Parse(pageCountArea[0].InnerText) / 15) + 1;

            for (int i = 1; i <= count; i++)
			{
                string orig_uri = target.Uri.OriginalString;
                string uriWithoutPage = orig_uri.Substring(0, orig_uri.Length - 1);
                Uri uri = new Uri(uriWithoutPage + i.ToString());
			    targets.Add(new DevourTarget(100, uri, new ItemsUrlReader()));

                Interlocked.Increment(ref _globalStudioCounter);
			}
            
            if (OnNewTargets != null)
            {
                OnNewTargets(targets);
            }
            //if (OnReadComplete != null)
            //{
            //    OnReadComplete();
            //}


            Console.WriteLine("{0} total page with DVD urls queued", _globalStudioCounter);
        }

        static int _globalStudioCounter;
    }
}
