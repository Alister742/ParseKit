using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using ParseKit.Data;
using System.IO;
using ParseKit.Parsers;
using ParseKit.CORE;

namespace ParseKit.ResourceClasses
{
    class KeysGiver
    {
        public static event EventHandler OnKeyPageParsed;

        public string[] ParseHightFK(string theme)
        {
            return null;
        }

        public string[] ParseAverageFK(string theme)
        {
            return null;
        }

        public string[] ParseLowFK(string theme)
        {
            return null;
        }

        public string[] ParseMixedFK(string theme)
        {
            return null;
        }

        public static List<string> ParseYandexKeys(int count, string startKey)
        {
            List<string> keys = new List<string>();
            keys.Add(startKey);

            string pattern = @"<td>\s*<a href=""\?cmd=words&amp;page=1&amp;ts=[^&]*&amp;key=[^&]*&amp;t=([^""]*)"">[^<]*</a>\s*</td>";
            string splitPattern = "<tr class=\"thead\" valign=\"bottom\">";
            string capchaPattern = @"<input type=""hidden"" name=""captcha_id"" value=""([^""]*)""[^>]*>";

            Regex rx = new Regex(pattern, RegexOptions.Compiled);
            Regex splitRx = new Regex(splitPattern, RegexOptions.Compiled);
            Regex capchaRx = new Regex(capchaPattern, RegexOptions.Compiled);

            int failTryCount = 0;
            int i = 0;
            CookieCollection cookies = new CookieCollection();
            while (count > keys.Count & i <= keys.Count - 1)
            {
                string key = Uri.EscapeUriString(keys[i]);
                string content = string.Empty;

                Uri keyUri = new Uri("http://wordstat.yandex.ru/?cmd=words&page=1&t=" + key + "&geo=&text_geo=");
                DownloaderObj obj = new DownloaderObj(keyUri, null, true, null, CookieOptions.UseShared & CookieOptions.Take, 5, null, cookies);
                Downloader.DownloadSync(obj);

                if (obj.DataStr == null & failTryCount < 5)
                {
                    failTryCount++;
                    continue;
                }
                else if (content == null) break;

                Match capchaResult = capchaRx.Match(content);
                if (capchaResult.Success)
                {
                    obj.Attempts = 3; obj.Uri = new Uri("http://kiks.yandex.ru/su/");
                    Downloader.HaveResponce(obj);
                    cookies = obj.Cookie;
                    continue;
                }

                content = splitRx.Split(content)[1];

                MatchCollection results = rx.Matches(content);

                if (results.Count <= 14)
                {
                    foreach (Match m in results)
                    {
                        keys.Add(Uri.UnescapeDataString(m.Groups[1].Value));
                    }
                }
                else
                {
                    for (int j = 1; j < 14; j++)
                    {
                        keys.Add(Uri.UnescapeDataString(results[j].Groups[1].Value));
                    }
                }
                keys = keys.Distinct().ToList<string>();
                i++;
                Console.WriteLine("ParseYandexKeys collect {0} keys and index is on {1} posotion...", keys.Count, i);
            }
            return keys;
        }

        public static List<string> ParseGoogleKeys(int count, string startKey)
        {
            List<string> keys = new List<string>();
            keys.Add(startKey);

            string pattern = "<p[^>]*><a href=\".*?q=([^&]*)&[^>]*?>.*?</a></p>";
            Regex rx = new Regex(pattern, RegexOptions.Compiled);
            
            int i = 0;
            while (count > keys.Count & i <= keys.Count - 1)
            {
                string key = Uri.EscapeUriString(keys[i].Replace(' ', '+'));

                Uri uri = new Uri("http://www.google.ru/search?q=" + key + "&sourceid=opera&num=0&ie=utf-8&oe=utf-8&start=0");

                DownloaderObj obj = new DownloaderObj(uri, null, true, null, CookieOptions.NoCookies, 3);
                Downloader.DownloadSync(obj);
                if (obj.DataStr == null) return null;

                MatchCollection results = rx.Matches(obj.DataStr);
                foreach (Match m in results)
                {
                    keys.Add(m.Groups[1].Value);
                }
                keys = keys.Distinct().ToList<string>();
                if (OnKeyPageParsed!=null) OnKeyPageParsed(null, new KeyEventArgs(keys.Count, i, count));
                i++;
            }
            return keys;
        }
    }

    class KeyWord
    {
        public string Name;
        public int Freq;

        public KeyWord(string name, int freq)
        {
            Name = name;
            Freq = freq;
        }
    }

    public class KeyEventArgs : EventArgs
    {
        public int KeysNeed;
        public int KeysCount;
        public int Position;

        public KeyEventArgs(int keysCount, int position, int keysNeed)
        {
            KeysCount = keysCount;
            Position = position;
            KeysNeed = keysNeed;
        }
    }
}
