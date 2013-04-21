using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using Core.ExternalTypes;

namespace Downloader
{
    public static class Downloader
    {
        const int ThreadCriticalBoundary = Config.DownloaderSet.ThreadBoundary;

        static Downloader()
        {
            ServicePointManager.DefaultConnectionLimit = ThreadCriticalBoundary;
            _parallelRequestsCap = Config.ParallelReqCap;
        }

        /// <summary>
        /// Value of maximum parallel requests which processing as parallel 
        /// </summary>
        public static int MaxParallelRequests
        {
            get { return _parallelRequestsCap; }
            set
            {
                if (value > ThreadCriticalBoundary)
                {
                    _parallelRequestsCap = ThreadCriticalBoundary;
                }
                else if (value < 0)
                {
                    _parallelRequestsCap = 0;
                }
                else
                    _parallelRequestsCap = value;
            }
        }

        /// <summary> 
        /// Count of total queued objects 
        /// </summary>
        public static int Queued { get { return _vipQueue.FullCount + _waitList.Count; } }
        /// <summary> 
        /// Count of objects currently processing 
        /// </summary>
        public static int Processing { get { return _hostsList.TotalCount; } }

        static List<HttpSession> _waitList = new List<HttpSession>();    // List of Downloader objects to proces
        static HashQueue<string> _vipQueue = new HashQueue<string>();       // Objects who will process when in _hostsList will no objects with the same hostname
        static KeyCountHashTable _hostsList = new KeyCountHashTable();      // Currently processing host names, no objects  

        static bool _suspended = false;
        static int _parallelRequestsCap;

        public static void Queue(HttpSession obj)
        {
            if (_hostsList.TotalCount < MaxParallelRequests)
            {
                ProcessObj(obj);
            }
            else
            {
                lock (_waitList)
                {
                    _waitList.Add(obj);
                }
            }
        }

        public static void Suspend()
        {
            _suspended = true;
        }

        public static void Resume()
        {
            _suspended = false;
            FullFillQueue();
        }

        public static void Abort()
        {
            throw new NotImplementedException();
        }

        public static void DropQueue()
        {
            _waitList.Clear();
            _vipQueue.Clear();
        }

        static void DequeueAndProcessObj()
        {
            HttpSession obj = GetRareHost();
            if (obj != null)
                ProcessObj(obj);
        }

        static void ProcessObj(HttpSession obj)
        {
            _hostsList.Add(obj.Uri.Host);
            //Console.WriteLine("left {0}", _objList.Count);
            new MagicClient().BeginReceive(obj);
        }

        internal static void ProcessNext(Uri lastUri)
        {
            if (_suspended)
                return;

            _hostsList.RemoveOneHost(lastUri.Host);

            if (_hostsList.TotalCount >= MaxParallelRequests)
                return;

            if (_hostsList.TotalCount < (MaxParallelRequests / 1.5))
            {
                FullFillQueue();
            }
            else
            {
                DequeueAndProcessObj();
            }
        }

        static void FullFillQueue()
        {
            while (_hostsList.TotalCount < MaxParallelRequests && _waitList.Count > 0)
            {
                DequeueAndProcessObj();
            }
        }

        public static bool HaveResponce(HttpSession obj)
        {
            return new MagicClient().HaveResponce(obj);
        }

        static HttpSession GetRareHost()
        {
            lock (_waitList)
            {
                lock (_hostsList)
                {
                    int minRate = _hostsList.TotalCount;
                    int minIndx = 0;
                    for (int i = 0; i < _waitList.Count; i++)
                    {
                        if (!_hostsList.ContainsKey(_waitList[i].Uri.Host))
                            return TakeAwayListedObj(i);

                        int curObjCount = _hostsList.GetCountByKey(_waitList[i].Uri.Host);
                        if (curObjCount < minRate)
                        {
                            minRate = curObjCount;
                            minIndx = i;
                        }
                    }

                    if (_waitList.Count > 0)
                        return TakeAwayListedObj(minIndx);
                    else
                        return null;
                }
            }
        }

        static HttpSession TakeAwayListedObj(int index) //ATTENTION NOT THREAD SAFE! 
        {
            HttpSession obj = _waitList[index];
            _waitList.RemoveAt(index);
            return obj;
        }

        #region Cookies
        public static void ClearCookies()
        {
            MagicClient.ClearCookies();
        }

        public static void SetCookies(CookieCollection cookies, Uri uri)
        {
            MagicClient.SaveCookies(uri, cookies);
        }

        public static CookieCollection GetCookies(DownloaderObj obj)
        {
            if (!obj.CookieOptions.HasFlag(CookieOptions.NoCookies))
            {
                return new MagicClient().GetCookies(obj);
            }
            else
                return null;
        }
        #endregion
    }
}
