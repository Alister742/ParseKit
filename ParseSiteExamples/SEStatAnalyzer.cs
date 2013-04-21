using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using ParseKit.Data;
using ParseKit.CORE;

namespace ParseKit
{
    class SEStatAnalyzer
    {
        
        public void AnalyzeSite(string address)
        {
            string yaIndx = @"<strong class=""b-head-logo__text"">[^<]*<br>([^<])</strong>";
            string googleIndx = @"<div id=resultStats>([^<])<nobr>[^<]*</nobr></div>";

            string bingIndx = @"<div class=""sb_ph""><span class=""sb_count"" id=""count"">([^<]*)</span></div>";

            string yaTic = @"<p class=""b-cy_error-cy"">[^—]*—([^<]*)</p>";
            string googlePR = @"<h3><span class=""blue"">G</span><span class=""red"">o</span><span class=""yellow"">o[^<p>]*<p><div class=""pull-right"">[^<]*<b>([^<]*)</b>";

            string ipAdr = @"<a id=""ipw""[^>]*>([^<]*)</a>";
            string hoster = @"<div class=""pull-right""><b>([^<]*)</b></div>";

            string host = Parse("http://www.pr-cy.ru/analysis/" + address, hoster);

            string yaIdx = Parse("http://yandex.ru/yandsearch?text=host:" + address, yaIndx);

            string googleIdx = Parse("http://www.google.ru/search?rls=en&q=site:" + address, googleIndx);

            string bingIdx = Parse("http://www.bing.com/search?q=site:" + address, bingIndx);

            string yaxTic = Parse("http://yaca.yandex.ru/yca/cy/ch/" + address, yaTic);

            string ipAdrs = Parse("http://www.cy-pr.com/analysis/" + address, ipAdr);

            string googlPR = Parse("http://pr-cy.ru/analysis/" + address, googlePR);
        }

        private static string Parse(string address, string pattern)
        {
            Uri uri = UriHandler.CreateUri(address);
            DownloaderObj obj = new DownloaderObj(uri, null, true, null, CookieOptions.UseShared & CookieOptions.SaveShared, 5);
            Downloader.DownloadSync(obj);
            if (obj.DataStr != null)
            {
                return (new Regex(pattern)).Match(obj.DataStr).Groups[1].ToString();
            }
            else return null;
        }
    }
}
