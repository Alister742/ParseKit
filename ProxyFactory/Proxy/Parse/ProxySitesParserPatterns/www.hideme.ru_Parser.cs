using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;




namespace ProxyFactory.Parser
{
    class HidemeParser : IProxySiteProvider
    {
        public List<RatedProxy> ParsePage(string data)
        {
            if (data == null) return null;
            List<RatedProxy> proxies = new List<RatedProxy>();
            Dictionary<string, string> imageLinksAndHash = new Dictionary<string, string>();

            string ipPattern = @"<td>(?<ip>[^<]*)</td><td><img src=""(?<image>/images/proxylist_port_\d*.gif)""></td>";
            Regex ipRx = new Regex(ipPattern);
            
            MatchCollection ipMatches = ipRx.Matches(data);

            Hashtable portHashes = LoadPortHashes();

            foreach (Match ipMatch in ipMatches)
            {
                string imagePath = "http://hideme.ru" + ipMatch.Groups["image"].Value;

                DownloaderObj obj = new DownloaderObj(new Uri(imagePath), null, false, null);
                Downloader.DownloadSync(obj);
                if (obj.Data == null) 
                    continue;

                string imageHash = GetMd5HashString(obj.Data);
                if (imageHash == null) 
                    continue;
                
                if (portHashes.Contains(imageHash))
                {
                    string port = portHashes[imageHash] as string;
                    string ip = ipMatch.Groups["ip"].Value;
                    
                    if (ip.IsValidIP() && port.IsValidPort())
                        proxies.Add(new RatedProxy(ip + ":" + port));
                }
                else
                {
                    if (!imageLinksAndHash.ContainsKey(imageHash))
                        imageLinksAndHash.Add(imageHash, imagePath);
                    continue;
                }
            }
            AddUnknownPortImage(imageLinksAndHash);
            return proxies;
        }

        private static void AddUnknownPortImage(IEnumerable imageLinksAndHash)
        {
            StreamWriter sw = new StreamWriter(PATH.UnknownHidemeHashes, false, Encoding.Default);

            foreach (KeyValuePair<string, string> linkAndHash in imageLinksAndHash)
            {
                sw.WriteLine("<p>{1}|&nbsp;<img src=\"{0}\" />", linkAndHash.Value, linkAndHash.Key);
            }
            sw.Dispose();
        }

        private static string GetMd5HashString(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            MD5 md5Hash = MD5.Create();

            if (data != null) md5Hash.ComputeHash(data);
            else return null;

            foreach (var b in md5Hash.Hash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        private static Hashtable LoadPortHashes()
        {
            StreamReader sr = new StreamReader(PATH.HidemeDotRuHashes, Encoding.Default);

            Hashtable portHashes = new Hashtable();
            while (!sr.EndOfStream)
            {
                string[] portHash = sr.ReadLine().Split('|');
                if (!portHashes.ContainsKey(portHash[0]))
                {
                    portHashes.Add(portHash[0], portHash[1]);
                }
            }
            sr.Dispose();
            return portHashes;
        }
    }
}
