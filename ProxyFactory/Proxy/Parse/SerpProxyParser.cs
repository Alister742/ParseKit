using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace ProxyFactory.Parser
{
    class SerpProxyParser //may be static bc no difference there betwen nonstatic and static
    {
        public delegate void UrlsPrgChangedDel(int proxyConunt, string adress);
        public event UrlsPrgChangedDel OnUrlsPrsProgrChanged;
        public delegate void SerpParseCompleteDel(List<RatedProxy> proxies);
        public event SerpParseCompleteDel OnUrlsOrSerpParseComplete;

        public List<RatedProxy> SerpProxy = new List<RatedProxy>();

        public void ParseProxyFromUrlsOrSerp(List<string> uriList)
        {
            List<Uri> validUris = new List<Uri>();
            foreach (var uri in uriList)
            {
                Uri validUri = UriHandler.CreateUri(uri);
                if (validUri != null) validUris.Add(validUri);
            }
            if (validUris.Count == 0) validUris = null;
            ParseProxyFromUrlsOrSerp(validUris);
        }

        public void ParseProxyFromUrlsOrSerp(List<Uri> uriList = null, bool onlyFromList = false)
        {
            if (uriList == null)
            {
                List<Uri> prxSites = new List<Uri>();

                //prxSites.AddRange(SESerpParser.ParseGoogleSerp(64, "http+proxy+list"));
                //prxSites.AddRange(SESerpParser.ParseYandexSerp(104, "прокси"));

                StreamWriter sw = new StreamWriter(PATH.Temp + @"\sites", true, Encoding.Default);
                foreach (Uri uri in prxSites)
                {
                    sw.WriteLine(uri);
                }
                sw.Dispose();

                ParseProxyFromUrlsOrSerp(prxSites);
            }
            else
            {
                SerpProxy.Clear();
                //Devourer pagesGiver = new Devourer();
                //pagesGiver.OnPageDownloaded += new Devourer.PageDownloadedDelegate(ParseSerpData);
                //pagesGiver.OnDownloadComplete += new Devourer.DownloadCompleteDel(pagesGiver_OnDownloadComplete);
                //pagesGiver.GiveSitesPagesUrls(uriList, onlyFromList);
            }
        }

        private void ParseSerpData(DownloaderObj obj)
        {
            List<RatedProxy> p = ProxyParser.ParseProxy(obj.DataStr);
            if (p != null)
            {
                lock (SerpProxy)
                {
                    SerpProxy.AddRange(p);
                }
                if (OnUrlsPrsProgrChanged != null) OnUrlsPrsProgrChanged(p.Count, obj.Uri.OriginalString);
            }
        }

        private void pagesGiver_OnDownloadComplete(List<Uri> parsedUrls)
        {
            if (OnUrlsOrSerpParseComplete != null) OnUrlsOrSerpParseComplete(SerpProxy);
        }
    }
}
