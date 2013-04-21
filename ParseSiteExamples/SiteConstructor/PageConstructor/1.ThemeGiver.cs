using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using ParseKit.Proxy;
using System.Web;
using System.Threading;
using ParseKit.Data;
using ParseKit.CORE;
using ParseKit.Utils;

namespace ParseKit.ResourceClasses
{
    class ThemeGiver
    {
        List<string[]> popThemes = new List<string[]>();
        List<string[]> otherThemes = new List<string[]>();
        List<string[]> allThemes = new List<string[]>();

        static Random random = new Random();

        public ThemeGiver()
        {
            LoadThemes(PATH.Themes);
        }

        private void LoadThemes(string p)
        {
            using (StreamReader sr = new StreamReader(p, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    string[] theme = sr.ReadLine().Split('|');

                    theme[0].Replace(" ", "");
                    if (string.IsNullOrEmpty(theme[2]))
                        theme[2] = "0";

                    if (Int32.Parse(theme[1]) > 0)
                        popThemes.Add(theme);
                    else
                        otherThemes.Add(theme);
                    allThemes.Add(theme);
                }
            }
        }

        public string GetRandomTheme()
        {
            if (random.Next(0, popThemes.Count + otherThemes.Count) <= popThemes.Count)
                return popThemes[random.Next(0, popThemes.Count)][0];
            else
                return otherThemes[random.Next(0, otherThemes.Count)][0];
        }

        public string GetPopTheme()
        {
            return popThemes[random.Next(0, popThemes.Count)][0];
        }

        public string GetBestTheme()
        {
            int maxWeight = 0;
            string bestTheme = string.Empty;
            foreach (string[] thm in popThemes)
            {
                if (Int32.Parse(thm[1]) >= maxWeight)
                {
                    maxWeight = Int32.Parse(thm[1]);
                    bestTheme = thm[0];
                }
            }
            return bestTheme;
        }

        public void ResetWordsWeight()
        {
            foreach (string[] theme in allThemes)
            {
                theme[2] = "0";
            }
            SetRamblerWordsWeight();
            SetYandexWordsWeight();
        }

        private void SetYandexWordsWeight()
        {
            foreach (string[] theme in allThemes)
            {
                string escape = Uri.EscapeUriString(theme[0]);

                Uri pageUri = new Uri ("http://wordstat.yandex.ru/?cmd=words&page=1&t=" + escape + "&geo=&text_geo=");
                DownloaderObj obj = new DownloaderObj(pageUri, null);
                Downloader.DownloadSync(obj);

                string pattern = "<a href=\"?cmd=words&amp;page=1&amp;ts=[^;]*;key=[^;]*;t=" + escape + @""">[^<]*</a>\s*</td>\s*" +
                    @"<td><[^>]*></div>[^<]*</td>\s*" +
                    "<td[^>]*>([^<]*)</td>";
                theme[2] = (Int32.Parse(new Regex(pattern, RegexOptions.Compiled).Match(obj.DataStr).Groups[1].Value) + Int32.Parse(theme[2])).ToString();
                Thread.Sleep(Rnd.Next(4000, 6000));
            }
        }

        private void SetRamblerWordsWeight()
        {
            ProxyManager pm = new ProxyManager();

            string errorPattern = "<td class=\"error\">[^</td>]*</td></td>";
            Regex rx = new Regex(errorPattern);

            string req = GetRamblerAdStatReq();

            Uri ramblerUri = new Uri(req);
            DownloaderObj obj = new DownloaderObj(ramblerUri, null, true);
            Downloader.DownloadSync(obj);
            if (obj.DataStr == null) return;

            while (rx.IsMatch(obj.DataStr)) //write another method.....
            {
                //content = Content.DownloadString(req, pm.GetProxy());
            }

            if (!rx.IsMatch(obj.DataStr))
            {
                foreach (string[] theme in allThemes)
                {
                    string pattern = @"<tr>\s*<tr>\s*<td class=hd_gray><a href=[^>]*>" +
                        theme[0] +
                        @"</a>[^<]</td>\s*<td class=hd_gray[^>]*>([^<]*)</td>\s*" +
                        @"<td class=hd_gray[^>]*>[^<]*</td>\s*</tr>";

                    theme[2] = (Int32.Parse(new Regex(pattern).Match(obj.DataStr).Groups[1].Value.Replace("&nbsp;", "")) + Int32.Parse(theme[2])).ToString();
                }
            }
            else
            {
                //parse new proxies, and rerun method();
                //ProxyManager.
                GlobalLog.Write("Expired proxyes on SetRamblerWordsWeight() method, need to rerun method or get fresh proxies");
            } 
        }

        private string GetRamblerAdStatReq()
        {
            string dt;
            string themes;

            string m = string.Empty;
            int month = DateTime.Today.Month;
            if (month > 2) month -= 2;
            else if (month == 2) month = 12;
            else if (month == 1) month = 11;
            if (month < 10)
            {
                m = "0" + month;
            }
            dt = DateTime.Today.Year.ToString().Substring(2, 2) + m;

            themes = string.Empty;
            foreach (var theme in allThemes)
            {
                themes += "%0D%0A" + theme[0];
            }
            return "http://adstat.rambler.ru/wrds/?date=" + dt + "&words=" + themes + "&morph=0;sort=1";
        }
    }
}
