using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Parse.Regexp
{
    class RegexPatterns
    {
        public readonly Regex A_Href_Regex = new Regex("<a.*?href=[\"|\'](?<uri>[^\"|^\']*)[\"|\']", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        //public string[] GetAHrefUrls(string data)
        //{

        //}

        public HashSet<string> GetAHrefUrls(string page, Uri pageUri)
        {
            if (page == null)
                return null;
            HashSet<string> urls = new HashSet<string>();
            int founUrls = 0;
            MatchCollection curUrls = A_Href_Regex.Matches(page);

            foreach (Match m in curUrls)
            {
                string parsedUri = m.Groups["uri"].Value;
                Uri uri = UriHandler.CreateUri(parsedUri, pageUri.Scheme, pageUri.Host);
                if (uri != null)
                {
                    if (uri.Host == pageUri.Host && !urls.Contains(uri.OriginalString))
                    {
                        urls.Add(uri.OriginalString);
                        founUrls++;
                    }
                }
            }
            return urls;
        }
    }
}
