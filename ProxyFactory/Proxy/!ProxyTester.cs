using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace ProxyFactory
{
    class ProxyTester
    {
        #region Events
        public delegate void CompleteHandler();
        public delegate void ProxyTestHandler(RatedProxy proxy);
        public delegate void ProgressHandler(int left, bool unpingable);
        public event CompleteHandler OnTestsComplete;
        public event ProxyTestHandler OnProxyTestComplete;
        public event ProgressHandler OnProgressChanged;
        #endregion

        #region Variables Constants
        const int DownloadsAttempts = 4;
        int _proxiesTesting;
        bool _logging;
        #endregion

        ProxyTester(bool logging = false)
        {
            _logging = logging;
        }

        public bool HostReply(string host, int maxFails)
        {
            Ping ping = new Ping();
            byte[] buffer = new byte[32];
            double success = 0;
            for (int i = 0; i < maxFails; i++)
            {
                PingReply reply = ping.Send(host, 3000, buffer);
                if (reply.Status == IPStatus.Success)
                {
                    //proxy.AvgPing = (int)reply.RoundtripTime;
                    if (maxFails < 10) maxFails++;
                    success++;
                    Thread.Sleep(500);
                }
            }
            return (success / maxFails >= 0.5) ? true : false;
        }

        void TestByActions(List<RatedProxy> proxies, List<Action<RatedProxy>> tests)
        {
            if (proxies.Count == 0)
                return;
            _proxiesTesting = proxies.Count;

            foreach (var proxy in proxies)
            {
                RatedProxy tmpProxy = proxy;
                ThreadPool.QueueUserWorkItem((object o) =>
                {
                    if (!HostReply(tmpProxy.Address.Host, 5))
                        EndTest(tmpProxy, true);
                    else
                    {
                        foreach (var test in tests)
                        {
                            if (test != null)
                                test.Invoke(tmpProxy);
                        }
                    }
                });
            }
        }

        #region Test Methods
        public void ReplyTest(List<RatedProxy> proxies)
        {
            OptionalTest(proxies);
        }

        public void OptionalTest(List<RatedProxy> proxies, bool latSpeedAndRate = false, bool downloads = false, bool se = false, bool anonymous = false, bool rbl = false)
        {
            List<Action<RatedProxy>> tests = new List<Action<RatedProxy>>();

            if (latSpeedAndRate)
                tests.Add(VerifySites);

            if (downloads)
                tests.Add(BeginDownloads);

            if (se)
                tests.Add(DownloadGooglePage);

            if (anonymous)
                tests.Add(BeginCheckAnonymous);

            if (rbl)
                tests.Add(CheckRBL);

            TestByActions(proxies, tests);
        }

        public void SeTest(List<RatedProxy> proxies)
        {
            List<Action<RatedProxy>> tests = new List<Action<RatedProxy>>();
            tests.Add(DownloadGooglePage);
            TestByActions(proxies, tests);
        }

        public void RegularTest(List<RatedProxy> proxies)
        {
            OptionalTest(proxies, true, true);
        }

        public void FullTest(List<RatedProxy> proxies)
        {
            OptionalTest(proxies, true, true, true, true, true);
        }
        #endregion

        #region SE
        void DownloadGooglePage(RatedProxy proxy)
        {
            Uri googleUri = new Uri("http://www.google.com/search?q=flowers&sourceid=opera&ie=utf-8&oe=utf-8");
            DownloaderObj obj = new DownloaderObj(googleUri, CheckGooglePage, true, proxy, CookieOptions.Empty, 2);
            if (Downloader.HaveResponce(obj))
            {
                obj.Attempts = 2;
                Downloader.Queue(obj);
            }
            else
            {
                if (_logging)
                    GlobalLog.Err("Cant get responce from http://google.com, perhaps proxy or ya host is down, p:" + obj.Proxy.Address.Host);
            }
        }

        void CheckGooglePage(DownloaderObj obj)
        {
            bool noGoogleBan = PageIsOk(obj.DataStr, GlobalResourceCache.GooglePagePattern);
            obj.Proxy.GoogleRate = noGoogleBan ? 1 : 0;
            obj.Proxy.GoogleChecked++;
        }

        void DownloadYaPage(RatedProxy proxy)
        {
            Uri yandexUri = new Uri("http://kiks.yandex.ru/su/");
            DownloaderObj obj = new DownloaderObj(yandexUri, CheckYaPage, true, proxy, CookieOptions.Take, 2);
            if (Downloader.HaveResponce(obj))
            {
                obj.Uri = new Uri("http://wordstat.yandex.ru/");
                obj.Attempts = 2;
                Downloader.Queue(obj);
            }
            else
            {
                if (_logging)
                    GlobalLog.Err("Cant get responce from http://kiks.yandex.ru, perhaps proxy or ya host is down, p:" + obj.Proxy.Address.Host);
            }
        }

        void CheckYaPage(DownloaderObj obj)
        {
            bool noYaBan = PageIsOk(obj.DataStr, GlobalResourceCache.YaPagePattern);
            obj.Proxy.YaRate = noYaBan ? 1 : 0;
            obj.Proxy.YaChecked++;
        }

        bool PageIsOk(string page, SePatterns pagePattern)
        {
            bool notNull = page != null;
            bool ban = pagePattern.BanRx.IsMatch(page);
            bool ok = pagePattern.ValidRx.IsMatch(page);

            return notNull && !ban && ok;
        }
        #endregion

        #region Anonymous
        void BeginCheckAnonymous(RatedProxy proxy)
        {
            Uri anonymUri = new Uri("http://checker.samair.ru/");
            DownloaderObj obj = new DownloaderObj(anonymUri, EndCheckAnonimous, true, proxy, CookieOptions.NoCookies, 3);
            Downloader.Queue(obj);
        }

        void EndCheckAnonimous(DownloaderObj obj)
        {
            AnonymousRegexes anonRegxs = GlobalResourceCache.AnonymCheck;
            if (obj.DataStr != null)
            {
                if (anonRegxs.Anonymous.IsMatch(obj.DataStr))
                {
                    obj.Proxy.AnonymousLevel = AnonymousLevel.Anonymous;
                }
                else if (anonRegxs.HightAnonymous.IsMatch(obj.DataStr))
                {
                    obj.Proxy.AnonymousLevel = AnonymousLevel.HightAnonymous;
                }
                else
                    obj.Proxy.AnonymousLevel = AnonymousLevel.NotAnonymous;
            }
            else
            {
                if (_logging)
                    GlobalLog.Err("Cant download page from anon checking site, p:" + obj.Proxy.Address.Host);
            }
        }
        #endregion

        #region Latency DownloadSpeed SitesRate
        void VerifySites(RatedProxy proxy)
        {

            //DEBUG
            DateTime start = DateTime.Now; 

            int downloadCheked  = 0;
            int latencyCheked   = 0;
            double totalSiteRate = 0;
            double totalSpeed = 0;
            long totalLatency = 0;

            List<PatternsContainer> siteChecks = PagePatternGrabber.LoadPatterns(PATH.TagClassPatterns);

            foreach (var siteCheck in siteChecks)
            {
                double siteRate = 0;
                double downloadSpeed = 0;
                if (TryCheckDownloadSpeed(proxy, siteCheck.Uri, siteCheck.Validation, ref siteRate, ref downloadSpeed))
                {
                    totalSiteRate += siteRate;
                    totalSpeed += downloadSpeed;
                    downloadCheked++;
                } 
                
                int? latency = GetAvgLatency(siteCheck.Uri, proxy, 4);
                if (latency.HasValue)
                {
                    latencyCheked++;
                    totalLatency += latency.Value;
                }
            }
            if (latencyCheked > 0)
            {
                proxy.AvgLatency = (int)(totalLatency / latencyCheked);
            }
            if (downloadCheked > 0)
            {
                proxy.SitesRate = totalSiteRate / downloadCheked;
                proxy.AvgSpeed = totalSpeed / downloadCheked;
            }
            //DEBUG
            Console.WriteLine("VerifySites - elapsed {0}s, latency {1}, sites rate {2}, speed {3} ", (DateTime.Now - start).TotalSeconds, (int)(totalLatency / latencyCheked), totalSiteRate / downloadCheked, totalSpeed / downloadCheked); //DEBUG
            proxy.CheckTimes++;
        }

        private bool TryCheckDownloadSpeed(RatedProxy proxy, Uri uri, IPageValidator validator, ref double siteRate, ref double downloadSpeed)
        {
            Stopwatch timer = new Stopwatch();
            for (int i = 0; i < 3; i++)
            {
                DownloaderObj obj = new DownloaderObj(uri, null, true, proxy, CookieOptions.Empty, 1);
                timer.Restart();
                Downloader.DownloadSync(obj);
                timer.Stop();
                string data = obj.DataStr;
                if (data != null)
                {
                    if (validator.Validate(data))
                    {
                        siteRate = (1 - i / 3d);
                        int leng = Encoding.UTF8.GetBytes(data).Length;
                        downloadSpeed = Encoding.UTF8.GetBytes(data).Length / (double)timer.ElapsedMilliseconds; //KB-sec
                        return true;
                    }
                }
            }
            return false;
        }

        int? GetAvgLatency(Uri uri, RatedProxy proxy, int attempts)
        {
            Stopwatch timer = new Stopwatch();
            int totalLatency = 0;
            int score = 0;

            for (int i = 0; i < attempts; i++)
            {
                DownloaderObj obj = new DownloaderObj(uri, null, false, proxy, CookieOptions.Empty, 1);
                timer.Restart();
                bool haveResponse = Downloader.HaveResponce(obj);
                timer.Stop();

                if (haveResponse)
                {
                    totalLatency += (int)timer.ElapsedMilliseconds;
                    score++;
                }
            }
            if (score == 0)
                return null;

            return totalLatency / score;
        }
        #endregion

        #region RBL
        void CheckRBL(RatedProxy proxy)
        {
            DateTime start = DateTime.Now; //DEBUG

            List<string[]> rblList = GlobalResourceCache.RBLList;
            double rblBanRate = 0;

            int RBLChecks = 0;

            foreach (var rblSet in rblList)
            {
                Uri rblUri = new Uri(rblSet[0].Replace("*IP*", proxy.Address.Host));
                Regex positiveRx = new Regex(rblSet[1]);
                Regex negativeRx = new Regex(rblSet[2]);

                double rate = GetRblBanRate(rblUri, positiveRx, negativeRx, proxy);
                if (rate > -1)
                {
                    RBLChecks++;
                    rblBanRate += rate;
                }
            }

            if (RBLChecks > 0)
            {
                proxy.RBLBanRate = rblBanRate / RBLChecks;
            }
            Console.WriteLine("CheckRBL - elapsed {0}s, rate {1}", (DateTime.Now - start).TotalSeconds, rblBanRate / RBLChecks); //DEBUG
        }
        double GetRblBanRate(Uri rblUri, Regex positiveRx, Regex negativeRx, RatedProxy proxy)
        {
            DownloaderObj obj = new DownloaderObj(rblUri, null, true, proxy, CookieOptions.NoCookies, 4, null, null, false, 1000, new TimingParams(TimingPattern.BigWait));
            Downloader.DownloadSync(obj);

            if (obj.DataStr == null) 
                return -1;

            int blocked = negativeRx.Matches(obj.DataStr).Count;
            int noblocked = positiveRx.Matches(obj.DataStr).Count;

            bool noResults = (blocked == 0) && (noblocked == 0);
            if (noResults) 
                return -1;
            return (double)blocked / (noblocked + blocked);
        }

        //ASYNC(NOT TESTED)
        #region Async RBL
        //void BeginRBLCheck(RatedProxy proxy)
        //{
        //    List<string[]> rblList = GlobalResourceCache.RBLList;
        //    int indx = 0;
        //    double rblBanRate = 0;
        //    int rblChecks = rblList.Count;

        //    BeginRBLCheck(indx, rblBanRate, rblChecks, proxy);
        //}
        //void EndRBLCheck(DownloaderObj obj)
        //{
        //    object[] args = obj.Arg as object[];
        //    int indx = (int)args[0];
        //    double rblBanRate = (double)args[1];
        //    int rblChecks = (int)args[2];

        //    List<string[]> rblList = GlobalResourceCache.RBLList;
        //    string[] rblSet = rblList[indx];
        //    Regex positiveRx = new Regex(rblSet[1]);
        //    Regex negativeRx = new Regex(rblSet[2]);

        //    double rate = GetRblBanRate(obj.DataStr, positiveRx, negativeRx);
        //    if (rate > -1)
        //    {
        //        rblBanRate += rate;
        //    }
        //    else
        //        rblChecks--;

        //    indx++;
        //    if (indx < rblList.Count)
        //    {
        //        BeginRBLCheck(indx, rblBanRate, rblChecks, obj.PrxContainer);
        //    }
        //    else if (rblChecks > 0)
        //    {
        //        obj.PrxContainer.RBLBanRate = rblBanRate / rblChecks;
        //    }
        //}
        //double GetRblBanRate(string data, Regex positiveRx, Regex negativeRx)
        //{
        //    if (data == null)
        //        return -1;
        //    double blocked = negativeRx.Matches(data).Count;
        //    double ok = positiveRx.Matches(data).Count;

        //    bool noResults = blocked == 0 && ok == 0;

        //    if (noResults)
        //        return -1;
        //    if (blocked == 0)
        //        return 0;
        //    if (ok == 0)
        //        return 1;

        //    return blocked / ok;
        //}
        //void BeginRBLCheck(int indx, double rblBanRate, int rblChecks, RatedProxy proxy)
        //{
        //    List<string[]> rblList = GlobalResourceCache.RBLList;

        //    if (indx < rblList.Count)
        //    {
        //        string[] rblSet = rblList[indx];
        //        DownloaderObj obj = new DownloaderObj(new Uri(rblSet[0]), EndRBLCheck, true, proxy, CookieOptions.NoCookies, 3, new object[] { indx, rblBanRate, rblChecks });
        //        Downloader.Queue(obj);
        //    }
        //}
        #endregion
        #endregion

        #region DownloadsCheck
        void BeginDownloads(RatedProxy proxy)
        {
            //Console.WriteLine("Downloads START {0}", DateTime.Now.ToShortTimeString());
            List<PatternsContainer> sitePatterns = PagePatternGrabber.LoadPatterns(PATH.TagClassPatterns);
            SyncWaitObj waiter = new SyncWaitObj(0);

            foreach (var patt in sitePatterns)
            {
                object[] args = { patt, waiter, sitePatterns.Count };
                DownloaderObj obj = new DownloaderObj(patt.Uri, EndDownload, true, proxy, CookieOptions.NoCookies, DownloadsAttempts, args);
                Downloader.Queue(obj);
            }
        }

        void EndDownload(DownloaderObj obj)
        {
            object[] args = obj.Arg as object[];
            PatternsContainer sitePatt = args[0] as PatternsContainer;
            SyncWaitObj waiter = args[1] as SyncWaitObj;
            int siteChecksCount = (int)args[2];

            if (obj.DataStr != null)
            {
                bool originalPage = sitePatt.Validation.Validate(obj.DataStr);
                if (originalPage)
                    waiter.MultidownloadRate += (double)obj.Attempts / (DownloadsAttempts - 1);
            }
            
            if (Interlocked.Increment(ref waiter.Count) == siteChecksCount)
            {
                obj.Proxy.MultidownloadRate = waiter.MultidownloadRate / siteChecksCount;
                EndTest(obj.Proxy);

                ////DEBUG
                //Console.WriteLine("Downloads END {0}, rate ", DateTime.Now.ToShortTimeString(), obj.Proxy.MultidownloadRate);
            }
        }
        #endregion

        private void EndTest(RatedProxy proxy, bool unpingable = false)
        {
            int proxiesCount = Interlocked.Decrement(ref _proxiesTesting);
            if (OnProgressChanged != null) OnProgressChanged(proxiesCount, unpingable);
            if (OnProxyTestComplete != null) OnProxyTestComplete(proxy);
            if (proxiesCount == 0) OnTestsComplete();
        }

        private void Pause()
        {
            Thread.Sleep(Rnd.Next(2000, 4000));
        }
    }
}
