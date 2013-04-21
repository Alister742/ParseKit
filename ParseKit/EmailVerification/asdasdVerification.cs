using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParseKit.CORE;
using HtmlAgilityPack;

namespace ParseKit.EmailVerification
{
    class asdasdVerification
    {
        public IEnumerable<string> GetLettersLinks(string user)
        {
            if (string.IsNullOrEmpty(user))
                throw new ArgumentException("Bad user can't search in public folder");

            Uri uri = new Uri("http://asdasd.ru/?u=" + user);
            DownloaderObj obj = new DownloaderObj(uri, null, true);
            Downloader.DownloadSync(obj);

            if (string.IsNullOrEmpty(obj.DataStr))
	            throw new Exception("Downloaded data is null, cant get mail's page");
	        
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(obj.DataStr);

            HtmlNodeCollection letterLinks = doc.DocumentNode.SelectNodes("//table[@id='msg_list']/td[@class='subj']/div/div/a[@href]");

            for (int i = 0; i < letterLinks.Count; i++)
            {
                yield return "http://asdasd.ru" + letterLinks[i].GetAttributeValue("href", "");
            }
        }

        //private void GetLettersCallback(DownloaderObj obj)
        //{
        //    if (obj.DataStr == null)
        //        return;


        //}

    }
}
