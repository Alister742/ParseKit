using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using System.IO;
using System.Security.Cryptography;
using System.Collections;
using System.Threading;


namespace ProxyFactory.Parser
{
    static class ProxyParser
    {
        public delegate void ParseCompleteDel(int proxyConunt);
        public static event ParseCompleteDel OnParseComplete;

        public static List<RatedProxy> ParseProxyFromPage(Uri uri)
        {
            DownloaderObj obj = new DownloaderObj(uri, null, true);
            Downloader.DownloadSync(obj);
            if (obj.DataStr == null) 
                return null;
            return ParseProxy(obj.DataStr);
        }

        public static List<RatedProxy> ParseProxyFromFile(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.Default);
            string content = sr.ReadToEnd();
            sr.Dispose();
            return ParseProxy(content);
        }

        public static List<RatedProxy> ParseProxy(string content)
        {
            if (content == null) 
                return null;
            string pattern = @"(?<ip>\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})(:|\s)+(?<port>\d{1,5})";
            Regex r = new Regex(pattern, RegexOptions.Compiled);
          
            MatchCollection matches = r.Matches(content);
            if (matches.Count == 0) 
                return null;

            List<RatedProxy> pList = new List<RatedProxy>();
            foreach (Match ipport in matches)
            {
                string address = ipport.Groups["ip"].Value + ":" + ipport.Groups["port"].Value;
                pList.Add(new RatedProxy(address, -1));
            }
            if (OnParseComplete != null & pList.Count > 0) 
                OnParseComplete(pList.Count);

            return pList;
        }
    }
}
