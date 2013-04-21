using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Threading;



namespace ProxyFactory.Parser
{
    class ProxyPageGrabber
    {
        public delegate void PageParsedDel(List<RatedProxy> proxies);
        public event PageParsedDel OnPageGrabbed;
        public delegate void ParseCompletedDel();
        public event ParseCompletedDel OnParseCompleted;

        public void BigParse()
        {
            ParseProxyhttpNetAsync();
            ParseHidemeRuAsync();
            ParseHidemyassComAsync(50);
            ParseSpysRuAsync();
        }

        public void ParseProxyhttpNetAsync()
        {
            BeginDownloadPages(10, "http://proxyhttp.net/free-list/anonymous-server-hide-ip-address/[N]#proxylist", "[N]", new proxyHttpParser());
        }

        public void ParseHidemeRuAsync()
        {
            BeginDownloadPages(1, "http://hideme.ru/proxy-list/", "[N]", new HidemeParser());
        }

        public void ParseHidemyassComAsync(int pages)
        {
            if (pages > 50) pages = 50;
            BeginDownloadPages(pages, "http://hidemyass.com/proxy-list/[N]", "[N]", new HidemyassParser());
        }

        public void ParseSpysRuAsync()
        {
            IProxySiteProvider spysProvider = new SpysRuParser();
            BeginDownloadPages(20, "http://www.spys.ru/proxies[N]/", "[N]", spysProvider);
            BeginDownloadPages(1, "http://www.spys.ru/", "[N]", spysProvider);
        }

        public void BeginDownloadPages(int count, string uriStr, string replaseSubstr, IProxySiteProvider proxySiteProvider)
        {
            if (count == 0 || string.IsNullOrEmpty(uriStr) || string.IsNullOrEmpty(replaseSubstr) || proxySiteProvider == null)
                throw new ArgumentException("Bad argumenst");

            WaitObj waiter = new WaitObj(count);

            for (int i = 0; i < count; i++)
            {
                Uri uri = new Uri(uriStr.Replace(replaseSubstr, i.ToString()));
                DownloaderObj obj = new DownloaderObj(uri, EndDownloadAndParse, true, null, CookieOptions.NoCookies, 10, new object[] { proxySiteProvider, waiter });
                Downloader.Queue(obj);
            }
        }

        void EndDownloadAndParse(DownloaderObj obj)
        {
            object[] args = obj.Arg as object[];
            IProxySiteProvider proxySiteProvider = args[0] as IProxySiteProvider;
            WaitObj waiter = args[1] as WaitObj;

            List<RatedProxy> proxies = null;

            proxies = proxySiteProvider.ParsePage(obj.DataStr);

            NotifyAboutProgress(waiter, proxies);
        }

        private void NotifyAboutProgress(WaitObj waiter, List<RatedProxy> proxies)
        {
            if (OnPageGrabbed != null) OnPageGrabbed(proxies);
            if (Interlocked.Decrement(ref waiter.Count) == 0)
            {
                if (OnParseCompleted != null) OnParseCompleted();
            }
        }
    }
}
