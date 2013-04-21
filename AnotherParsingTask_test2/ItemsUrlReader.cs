using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.Data;
using HtmlAgilityPack;
using System.Threading;
using ParseKit.Interfaces;

namespace AnotherParsingTask_test2
{
    public class ItemsUrlReader : IPageReader
    {

        public event NewTargetsDel OnNewTargets;
        public event ReadCompleteDel OnReadComplete;

        public void ReadData(string data, DevourTarget target)
        {
            List<DevourTarget> targets = new List<DevourTarget>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(data);

            HtmlNodeCollection pageCountArea = doc.DocumentNode.SelectNodes("//div[@id='main']/div[@style]/div[@style]/div[@style]/a[@class='screenshot3']");

            foreach (var item in pageCountArea)
            {
                Uri uri = new Uri("http://www.sexvideoall.com/de/" + item.GetAttributeValue("href", ""));
                targets.Add(new DevourTarget(100, uri, new ItemReader()));

                Interlocked.Increment(ref _globalUriFounded);
            }

            if (OnNewTargets != null)
            {
                OnNewTargets(targets);
            }
            //if (OnReadComplete != null)
            //{
            //    OnReadComplete();
            //}
            Console.WriteLine("{0} total page queued for scrapping", _globalUriFounded);
        }

        static int _globalUriFounded;
    }
}
