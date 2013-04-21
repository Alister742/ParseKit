using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Threading;
using System.Collections;


namespace ProxyFactory
{
    class ProxyManager
    {
        static object sync = new object();

        #region Selections
        //public static List<RatedProxy> CustomSelect(int checkedTimes, double multidownloadRate, int sitesRate, int maxlatency,  int count)
        //{
        //    List<RatedProxy> proxies = LoadProxies().FindAll((RatedProxy p) => { return (
        //                p.CheckedTimes >= checkedTimes &&
        //                p.MultidownloadRate >= multidownloadRate &&
        //                p.SitesRate >= sitesRate &&
        //                p.Latency <= maxlatency);
        //    });

        //    return SelectMaxOrCount(proxies, count);
        //}

        public static List<RatedProxy> SelectAnonymousProxies(AnonymousLevel anonymity, int count, List<RatedProxy> listfrom = null)
        {
            List<RatedProxy> selectTarget = listfrom == null ? LoadProxies() : listfrom;
            List<RatedProxy> yaProxies = selectTarget.FindAll((RatedProxy p) => { return (p.AnonymousLevel == anonymity); });
            return SelectMaxOrCount(yaProxies, count);
        }

        public static List<RatedProxy> SelectFastProxies(int count, List<RatedProxy> listfrom = null)
        {
            List<RatedProxy> proxies = listfrom == null ? LoadProxies() : listfrom;
            SortBySpeed(proxies);
            return SelectMaxOrCount(proxies, count);
        }

        public static List<RatedProxy> SelectRBLClearProxies(int count, List<RatedProxy> listfrom = null)
        {
            List<RatedProxy> proxies = listfrom == null ? LoadProxies() : listfrom;
            SortByRBL(proxies);
            return SelectMaxOrCount(proxies, count);
        }

        public static List<RatedProxy> SelectGoogleProxy(int count, List<RatedProxy> listfrom = null)
        {
            List<RatedProxy> selectTarget = listfrom == null ? LoadProxies() : listfrom;
            List<RatedProxy> googleProxies = selectTarget.FindAll((RatedProxy p) => { return (p.GoogleRate > 0.5); });
            return SelectMaxOrCount(googleProxies, count);
        }

        public static List<RatedProxy> SelectYaProxy(int count, List<RatedProxy> listfrom = null)
        {
            List<RatedProxy> selectTarget = listfrom == null ? LoadProxies() : listfrom;
            List<RatedProxy> yaProxies = LoadProxies().FindAll((RatedProxy p) => { return (p.YaRate > 0.5); });
            return SelectMaxOrCount(yaProxies, count);
        }

        public static List<RatedProxy> SelectBestProxy(int count, List<RatedProxy> listfrom = null)
        {
            List<RatedProxy> selectTarget = listfrom == null ? LoadProxies() : listfrom;
            List<RatedProxy> workProxies = selectTarget.FindAll((RatedProxy p) => { return (
                p.CheckTimes > 0 &&  
                p.MultidownloadRate > 0.5 && 
                p.SitesRate > 0.5 && 
                p.AvgLatency < 2000); 
            });
            List<RatedProxy> sortedWorkProxies = SortBySpeed(workProxies);

            return SelectMaxOrCount(sortedWorkProxies, count);
        }
        #endregion

        #region Sorts
        static List<RatedProxy> SortByRBL(List<RatedProxy> proxies)
        {
            for (int j = 0; j < proxies.Count; j++)
            {
                for (int i = 1; i < proxies.Count; i++)
                {
                    if (proxies[i - 1].RBLBanRate > proxies[i].RBLBanRate)
                    {
                        RatedProxy temp = proxies[i - 1];
                        proxies[i - 1] = proxies[i];
                        proxies[i] = temp;
                    }
                }
            }
            return proxies;
        }

        static List<RatedProxy> SortBySpeed(List<RatedProxy> proxies)
        {
            for (int j = 0; j < proxies.Count; j++)
            {
                for (int i = 1; i < proxies.Count; i++)
                {
                    if (proxies[i - 1].AvgSpeed < proxies[i].AvgSpeed)
                    {
                        RatedProxy temp = proxies[i - 1];
                        proxies[i - 1] = proxies[i];
                        proxies[i] = temp;
                    }
                }
            }
            return proxies;
        }
        #endregion

        public static void RemoveDuplicates()
        {
            List<RatedProxy> proxies = LoadProxies();
            Hashtable uniqueHosts = new Hashtable();
            foreach (var proxy in proxies)
            {
                if (!uniqueHosts.ContainsKey(proxy.Address.OriginalString))
                {
                    uniqueHosts.Add(proxy.Address.OriginalString, proxy);
                }
                else
                {
                    RatedProxy rp = uniqueHosts[proxy.Address.OriginalString] as RatedProxy;
                    if (proxy.CheckTimes > rp.CheckTimes)
                        uniqueHosts[proxy.Address.OriginalString] = proxy;
                }
            }
            RatedProxy[] uniqueHostsArray = new RatedProxy[uniqueHosts.Values.Count];
            uniqueHosts.Values.CopyTo(uniqueHostsArray, 0);

            SaveProxies(uniqueHostsArray.ToList(), false);
        }

        public static void RemoveRetardProxies()
        {
            List<RatedProxy> proxies = LoadProxies().FindAll((RatedProxy p) =>
            {
                return (
                    p.CheckTimes > 0 &&
                    p.AvgLatency < 2000 &&
                    p.MultidownloadRate >= 0.4 &&
                    p.SitesRate >= 0.5
                    ||
                    p.CheckTimes == 0);
            });
            ProxyManager.SaveProxies(proxies, false);
        }

        public static List<RatedProxy> SelectMaxOrCount(List<RatedProxy> proxies, int count)
        {
            count = count <= proxies.Count ? count : proxies.Count;
            return proxies.GetRange(0, count);
        }

        public static List<RatedProxy> LoadFewProxies(int count)
        {
            return SelectMaxOrCount(LoadProxies(), count);
        }

        public static List<RatedProxy> LoadProxies()
        {
            if (GlobalResourceCache.Proxies != null)
                return GlobalResourceCache.Proxies;

            List<RatedProxy> proxies = new List<RatedProxy>();
            lock (sync)
            {
                StreamReader sr = new StreamReader(PATH.Proxy, Encoding.Default);

                while (!sr.EndOfStream)
                {
                    string[] proxySet = null;
                    string adress = null;
                    int regularCheckTimes = 0;
                    double sitesRate = RatedProxy.DefaultVal;
                    double rblBanRate = RatedProxy.DefaultVal;
                    int latency = RatedProxy.DefaultVal;
                    int downloadSpeed = RatedProxy.DefaultVal;
                    double multiDownloadRate = RatedProxy.DefaultVal;
                    double yaRate = RatedProxy.DefaultVal;
                    int yaChecked = 0;
                    double googleRate = RatedProxy.DefaultVal;
                    int googleChecked = 0;
                    int anonymousLevel = 0;

                    try
                    {
                        proxySet = sr.ReadLine().Split('|');
                        adress = proxySet[0];
                        if (string.IsNullOrEmpty(adress)) 
                            continue;

                        Int32.TryParse(proxySet[1], out regularCheckTimes);
                        sitesRate = double.TryParse(proxySet[2], out sitesRate) ? sitesRate : -1;
                        rblBanRate = double.TryParse(proxySet[3], out rblBanRate) ? rblBanRate : -1;
                        Int32.TryParse(proxySet[4], out anonymousLevel);
                        latency = Int32.TryParse(proxySet[5], out latency) ? latency : -1;
                        downloadSpeed = Int32.TryParse(proxySet[6], out downloadSpeed) ? downloadSpeed : -1;
                        multiDownloadRate = double.TryParse(proxySet[7], out multiDownloadRate) ? multiDownloadRate : -1;
                        yaRate = double.TryParse(proxySet[8], out yaRate) ? yaRate : -1;
                        Int32.TryParse(proxySet[9], out yaChecked);
                        googleRate = double.TryParse(proxySet[10], out googleRate) ? googleRate : -1;
                        Int32.TryParse(proxySet[11], out googleChecked);
                    }
                    catch (Exception)
                    {  
                    }

                    proxies.Add(new RatedProxy(adress,
                            regularCheckTimes,
                            sitesRate,
                            rblBanRate,
                            (AnonymousLevel)anonymousLevel,
                            latency,
                            downloadSpeed,
                            multiDownloadRate,
                            yaRate,
                            yaChecked,
                            googleRate,
                            googleChecked));
                }
                sr.Dispose();
            }
            GlobalResourceCache.Proxies = proxies; //CACH
            return proxies;
        }

        public static void SaveProxies(List<RatedProxy> proxies, bool append = true)
        {
            if (proxies == null) 
                return;

            lock (sync)
            {
                using (StreamWriter sw = new StreamWriter(PATH.Proxy, append, Encoding.Default))
                {
                    foreach (RatedProxy p in proxies)
                    {
                        sw.WriteLine("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}",
                            p.Address.Host + ":" + p.Address.Port,
                            p.CheckTimes,
                            p.SitesRate,
                            p.RBLBanRate,
                            (int)p.AnonymousLevel,
                            p.AvgLatency,
                            p.AvgSpeed,
                            p.MultidownloadRate,
                            p.YaRate,
                            p.YaChecked,
                            p.GoogleRate,
                            p.GoogleChecked);
                    }
                }
            }
        }
    }
}
