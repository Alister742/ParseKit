using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using ParseKit.Data;
using ParseKit.ResourceClasses;
using ParseKit.Proxy;
using ParseKit.CORE;

namespace ParseKit.Parsers
{
    /* se regexes 
     *      Regex googleRx = new Regex(@"<h3 class=""r""><a\s*href=""/url\?q\=(?<url>[^""]*)""");
            Regex yaRx = new Regex(@"<a class=""b-serp-item__title-link"" href=""(?<url>[^""]*)""");
            Regex bingRx = new Regex(@"<div class=""sb_tlst""><h3><a href=""(?<url>[^""]*)""");
     */
    class SESerpParser
    {
        public delegate void ParseCompletedDel(List<string> urls);
        public event ParseCompletedDel OnParsed;
        public delegate void NotifyDel(int left);
        public event NotifyDel OnProgressChanged;
        public event EventHandler OnCompleted;

        public void ParseGoogleSerp(int keysCount, string firstKey)
        {
            string pattern = "<a href=\"([^\"]*)\" target=_blank class=l onmousedown=\"return[^>]*>.*?</a>";
            List<string> keys = KeysGiver.ParseGoogleKeys(keysCount, firstKey);
            QueueGooglePages(pattern, keys);
        }

        private void QueueGooglePages(string pattern, List<string> keys)
        {
            Regex rx = new Regex(pattern);
            WaitObj waiter = new WaitObj(keys.Count * 50);
            ///ProxyRotator proxyGiver = new ProxyRotator(ProxyManager.LoadProxies());

            foreach (var key in keys)
            {
                for (int i = 0; i < 50; i++)
                {
                    Uri uri = new Uri("http://www.google.ru/search?q=" + key + "&sourceid=opera&num=0&ie=utf-8&oe=utf-8&start=" + i);
                    //DownloaderObj obj = new DownloaderObj(uri, EndGetPage, true, null, CookieOptions.NoCookies, 10, new object[] { rx, waiter, proxyGiver }, null, false, 3000);
                    //Downloader.Queue(obj);
                }
            }
        }

        public void ParseYandexSerp(int keysCount, string firstKey)
        {
            //проблема: забанили по ип/ решение: взять куки с чистого ипа

            string pattern = @"<a class=""b-serp-item__title-link"" href=""(?<url>[^""]*)""";
            List<string> keys = KeysGiver.ParseYandexKeys(keysCount, firstKey);
            ParseSerpAsync(pattern, keys, GetYandexPage);
        }

        //public void QueueGooglePages(int numPage, string key, Regex rx, WaitObj waiter)
        //{
        //    Uri uri = new Uri("http://www.google.ru/search?q=" + key + "&sourceid=opera&num=0&ie=utf-8&oe=utf-8&start=" + numPage);
        //    DownloaderObj obj = new DownloaderObj(uri, EndGetPage, true, null, false, 10, rx);
        //    Downloader.Queue(obj);
        //}

        void EndGetPage(DownloaderObj obj)
        {
            object[] args = obj.Arg as object[];

            if (obj.DataStr != null)
            {
                List<string> urls = new List<string>();
                Regex rx = args[0] as Regex;
                WaitObj waiter = args[1] as WaitObj;
                MatchCollection urlsMatches = rx.Matches(obj.DataStr);
                foreach (Match urlMatch in urlsMatches)
                {
                    urls.Add(urlMatch.Groups[1].Value);
                }
                if (OnParsed!=null) OnParsed(urls);
                if (Interlocked.Decrement(ref waiter.Count) == 0 && OnCompleted != null) OnCompleted(this, EventArgs.Empty);
            }
            else
            {
                //ProxyRotator proxyGiver = args[2] as ProxyRotator;
                //proxyGiver.TryGetProxy(ref obj.PrxContainer);
                //obj.Proxies = proxyGiver;
                obj.Attempts = 10;
                obj.CallBack = EndGetPageWithProxy;
            }
        }

        void EndGetPageWithProxy(DownloaderObj obj)
        {

        }

        void GetYandexPage(int numPage, string key, Regex rx, WaitObj waiter)
        {
            Uri pageUri = new Uri("http://yandex.ru/yandsearch?p=" + numPage + "&text=" + Uri.EscapeDataString(key));
            DownloaderObj obj = new DownloaderObj(pageUri, null);
            Downloader.DownloadSync(obj);
        }

        void ParseSerpAsync(string uriPattern, List<string> keys, Action<int, string, Regex, WaitObj> requestgiver)
        {
            List<Uri> urls = new List<Uri>();
            string pattern = uriPattern;
            Regex rx = new Regex(pattern);
            List<string> keyPages = new List<string>();

            WaitObj waiter = new WaitObj(keys.Count * 50);
            for (int i = 0; i < keys.Count; i++)
            {
                for (int p = 0; p < 50; p++)
                {
                    requestgiver.Invoke(p, keys[i], rx, waiter);
                }
            }
        }
    }
}














        //static public List<Uri> ParseGoogleSerp(int keysCount, string firstKey)
        //{
        //    List<Uri> sites = new List<Uri>();
        //    string pattern = "<a href=\"([^\"]*)\" target=_blank class=l onmousedown=\"return[^>]*>.*?</a>";
        //    Regex rx = new Regex(pattern);

        //    List<string> keys = KeysGiver.ParseGoogleKeys(64, "http+proxy+list");

        //    int pagesCount = keys.Count * 50;
        //    int failTry = 0;
        //    foreach (string key in keys)
        //    {
        //        int idx = keys.IndexOf(key) * 50;

        //        for (int i = 0; i < 50; i++)
        //        {
        //            string content = Content.DownloadString("http://www.google.ru/search?q=" + key + "&hl=en&rls=en&start=" + i + "0&sa=N", null, false);
        //            if (content == null & failTry < 10)
        //            {
        //                i--;
        //                failTry++;
        //                Console.WriteLine("Now we get null content, try to wait {0} second, and retry current query.\n We will be increase pause time on 1 sec, if pause time will be right then we will be use that time pause every query...", 150000);
        //                Thread.Sleep(Elizabet.random.Next(156000, 160000));
        //                continue;
        //            }
        //            else if (content == null)
        //            {
        //                failTry = 0;
        //                continue;
        //            }

        //            MatchCollection proxySites = rx.Matches(content);
        //            foreach (Match item in proxySites)
        //            {
        //                sites.Add(Content.CreateValidUri(item.Groups[1].Value));
        //            }

        //            //debug
        //            Console.Clear();
        //            Console.WriteLine("ParseGoogleProxyUrlsp parsing proxy sites urls... Parsed {0} of {1} pages. {2} results was added", idx + i, pagesCount, sites.Count);
        //        }
        //    }
        //    return sites;
        //}

        //static public List<Uri> ParseYandexSerp(int keysCount, string firstKey)
        //{
        //    List<Uri> sites = new List<Uri>();

        //    //<a class="b-serp-item__title-link" href="http://www.asd.ru/" onmousedown="" target="_blank" tabindex="2"><span><b>ASD</b> и Позитроника: ...и цифровая техника - Позитроника, <b>ASD</b>-сервис...</span></a>
        //    //<a class="b-serp-item__title-link" href="([^"]*)" onmousedown="[^"]*"[^>]*>.*?</a>

        //    string pattern = "<a class=\"b-serp-item__title-link\" href=\"([^\"]*)\" onmousedown=\"[^\"]*\"[^>]*>.*?</a>";
        //    Regex rx = new Regex(pattern);

        //    List<string> keys = KeysGiver.ParseYandexKeys(104, "прокси");
        //    int pagesCount = keys.Count * 50;
        //    int failTry = 0;
        //    foreach (string key in keys)
        //    {
        //        int idx = keys.IndexOf(key) * 50;
        //        for (int i = 0; i < 50; i++)
        //        {
        //            string content = Content.DownloadString("http://yandex.ru/yandsearch?p=" + i + "&text=" + Uri.EscapeDataString(key));
        //            if (content == null & failTry < 10)
        //            {
        //                i--;
        //                failTry++;
        //                Console.WriteLine("Now we get null content, try to wait {0} second, and retry current query.\n We will be increase pause time on 1 sec, if pause time will be right then we will be use that time pause every query...", 150000);
        //                Thread.Sleep(Elizabet.random.Next(156000, 160000));
        //                continue;
        //            }
        //            else if (content == null)
        //            {
        //                failTry = 0;
        //                continue;
        //            }

        //            MatchCollection proxySites = rx.Matches(content);
        //            foreach (Match item in proxySites)
        //            {
        //                sites.Add(Content.CreateValidUri(item.Groups[1].Value));
        //            }

        //            //debug
        //            Console.Clear();
        //            Console.WriteLine("ParseYandexProxyUrls parsing proxy sites urls... Parsed {0} of {1} pages. {2} results was added", idx + i, pagesCount, sites.Count);
        //        }
        //    }
        //    return sites;
        //}