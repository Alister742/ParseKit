using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using System.Threading;


namespace ProxyFactory.Parser
{
    class HidemyassParser : IProxySiteProvider
    {
        string pattern = @"<td><span><style>\s*" +
                        @"(\.[^{]*{display:[^}]*}\s*)*" +
                        @"</style>(?<ip>.*?)</td>\s*" +
                        @"<td>\s*(?<port>[^<]*)</td>";

        string classPattern = @"(\.[^{]*){display:none}\s*";
        string ipSplitPattern = "</[^>]*>";
        string tagInvalidPattern = @"<[^>]*>\w*";
        string tagValidPattern = @"<[^>]*>";

        Regex rx;
        Regex classRx;
        Regex ipRx;
        Regex ipInvalidReplaceRx;
        Regex ipValidReplaceRx;

        public HidemyassParser()
        {
            rx = new Regex(pattern, RegexOptions.Compiled);
            classRx = new Regex(classPattern);
            ipRx = new Regex(ipSplitPattern);
            ipInvalidReplaceRx = new Regex(tagInvalidPattern);
            ipValidReplaceRx = new Regex(tagValidPattern);
        }

        public List<RatedProxy> ParsePage(string data)
        {
            if (data == null) return null;
            List<RatedProxy> proxies = new List<RatedProxy>();
            string nonedisplayClasses = string.Empty;

            MatchCollection matches = rx.Matches(data);

            foreach (Match m in matches)
            {
                string ip = string.Empty;
                string port = m.Groups["port"].Value;
                string ipString = m.Groups["ip"].Value;

                string classesString = m.Value;
                MatchCollection classes = classRx.Matches(classesString);
                foreach (Match mth in classes)
                {
                    nonedisplayClasses += mth.Groups[1].Value;
                }
                string[] noneDisplayClasses = nonedisplayClasses.Split('.').Skip(1).ToArray<string>();

                string[] ipFragments = ipRx.Split(ipString);
                for (int j = 0; j < ipFragments.Length; j++)
                {
                    if (ipFragments[j].Contains("display:none"))
                    {
                        ipFragments[j] = ipInvalidReplaceRx.Replace(ipFragments[j], "");
                    }
                    else
                    {
                        foreach (string cl in noneDisplayClasses)
                        {
                            if (ipFragments[j].Contains(cl))
                            {
                                ipFragments[j] = ipInvalidReplaceRx.Replace(ipFragments[j], "");
                            }
                        }
                    }
                    ip += ipValidReplaceRx.Replace(ipFragments[j], "");
                }
                try
                {
                    if (ip.IsValidIP() && port.IsValidPort())
                        proxies.Add(new RatedProxy(ip + ":" + port)); 
                }
                catch (Exception e)
                {
                }

            }
            return proxies;
        }
    }
}