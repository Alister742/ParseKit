using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Threading;

namespace ProxyFactory
{
    public class ProxyContainer : IProxyContainer
    {
        object sync = new object();

        public RatedProxy Proxy { get { return _proxy; } }
        public bool Fired { get; set; }
        public int Rating
        {
            get { lock (sync) return _rating; }
            set { lock (sync) _rating = value; }
        }
        /// <summary>
        /// Max count of possible simultaneously usings 
        /// </summary>
        public int MaxOccupied
        {
            get { lock (sync) return _maxOccups; }
            set { lock (sync) _maxOccups = value; }
        }
        /// <summary>
        /// Current count of objects using this proxy at the same time
        /// </summary>
        public int OccupiedTimes
        {
            get { lock (sync) return _occupied; }
            set { lock (sync) _occupied = value; }
        }
        public int Lifes
        {
            get { lock (sync) return _lifes; }
            set { lock (sync) _lifes = value; }
        }
        public bool Busy 
        {
            get 
            { 
                lock (sync) 
                    return _occupied >= _maxOccups ? true : false; 
            }
        }

        int _occupied;
        int _maxOccups;
        int _rating;
        int _lifes;
        RatedProxy _proxy;

        public ProxyContainer(RatedProxy proxy, int maxOccupied, int maxLifes)
        {
            _proxy = proxy;
            _maxOccups = maxOccupied;
            _lifes = maxLifes;
        }
    }

    public class ProxyProvider : IProxyProvider
    {
        public int SlotsAvailable
        {
            get
            {
                int slots = 0;
                _proxies.ForEach((p) => { slots += p.MaxOccupied - p.OccupiedTimes; });
                return slots;
            }
        }

        //public event ProxyReleaseDel OnFreeProxies;
        public event FreeProxyDel OnProxyFreed;
        public event EventHandler OnProxiesResulted;

        #region Localhost Param
        public bool UseLocalhost { get { return _useLocalhost; } }
        const int _localhost_lives = Config.Lh_lives;
        const int _localhost_occupation = Config.Lh_occupation;
        #endregion

        int _workingCount
        {
            get
            {
                int counter = 0;
                foreach (var proxy in _proxies)
                {
                    if (!proxy.Fired)
                        counter++;
                }
                return counter;
            }
        }

        List<ProxyContainer> _proxies = new List<ProxyContainer>();
        int _maxOccupiedTimes;
        int _prxLifes;
        bool _useLocalhost;
        //int _workingCount;

        /// <summary>
        /// Init new ProxyProvider
        /// </summary>
        /// <param name="proxies">if param is null then using only localhost</param>
        /// <param name="prx_maxOccupiedTimes"></param>
        /// <param name="prxLifes">Count of errors proxy can resist</param>
        /// <param name="useLocalhost"></param>
        public ProxyProvider(List<RatedProxy> proxies, int prx_maxOccupiedTimes = 1, int prxLifes = 3, bool useLocalhost = true)
        {
            proxies = proxies ?? new List<RatedProxy>();

            _maxOccupiedTimes = prx_maxOccupiedTimes;
            _prxLifes = prxLifes;
            _useLocalhost = useLocalhost;

            FillProxyList(proxies);
        }

        void FillProxyList(List<RatedProxy> proxies)
        {
            proxies.ForEach((cur_p) => 
            {
                ProxyContainer p_container = new ProxyContainer(cur_p, _maxOccupiedTimes, _prxLifes);

                if (_proxies.Count == 0)
                {
                    _proxies.Add(p_container);
                }
                else
                {
                    bool inserted = false;
                    for (int i = 0; i < _proxies.Count; i++)
                    {
                        ProxyContainer selec_cont = _proxies[i];
                        RatedProxy selected_p = selec_cont.Proxy;
                        
                        bool speedBetter = cur_p.AvgSpeed > selected_p.AvgSpeed;
                        bool siteRateBetter = cur_p.SitesRate > selected_p.SitesRate;
                        bool latencyBetter = (cur_p.AvgLatency != RatedProxy.DefaultVal && cur_p.AvgLatency < selected_p.AvgLatency);
                        bool cur_p_better = speedBetter || siteRateBetter || latencyBetter;
                        
                        if (cur_p_better)                                                   /* better proxies will be on the top of list */
                        {
                            p_container.Rating = ++selec_cont.Rating;
                            _proxies.Insert(i, p_container);
                            inserted = true;
                            break;
                        }
                    }

                    if (!inserted)
                    {
                        p_container.Rating = _proxies.Last().Rating;
                        _proxies.Add(p_container);
                    }
                }
            });

            if (_useLocalhost)
            {
                ProxyContainer localhost = new ProxyContainer(null, _localhost_occupation, _localhost_lives);
                _proxies.Insert(0, localhost);

                localhost.Rating = 999;
            }
        }

        public void Fire(ProxyContainer proxy)
        {
            proxy.MaxOccupied = 1;
            if (!(--proxy.Lifes > 0))
            {
                proxy.Fired = true;
                if (_workingCount == 0)//(Interlocked.Decrement(ref _workingCount) <= 0)
                {
                    if (OnProxiesResulted != null)
                        OnProxiesResulted(this, EventArgs.Empty);
                }
            }
        }

        public bool TryGet(out ProxyContainer proxy)
        {
            foreach (var p in _proxies)
            {
                if (!p.Fired && !p.Busy)
                {
                    p.OccupiedTimes++;
                    proxy = p;
                    return true;
                }
            }
            proxy = null;
            return false;
        }

        public void Release(ProxyContainer proxy, bool success)
        {
            if (success)
            {
                proxy.Rating++;
            }
            else
            {
                lock (proxy)
                {
                    if (proxy.MaxOccupied > 1)
                        proxy.MaxOccupied--;
                }
                proxy.Rating--;
            }

            proxy.OccupiedTimes--;
            CorrectPosition(proxy);

            if (!proxy.Busy)
            {
                OnProxyFreed(proxy);
            }
        }

        void CorrectPosition(ProxyContainer proxy)
        {
            int indx = _proxies.IndexOf(proxy);
            int prev = indx - 1;
            int next = indx + 1;

            if (prev > 0) /* if proxy not rank 1 */
            {
                ProxyContainer prx_prev = _proxies[prev];
                if (prx_prev.Rating < proxy.Rating)
                    SwitchProxies(prx_prev, proxy);
            }
            else if (next < _proxies.Count)
            {
                ProxyContainer prx_next = _proxies[next];
                if (prx_next.Rating > proxy.Rating)
                    SwitchProxies(prx_next, proxy);
            }
        }

        void SwitchProxies(ProxyContainer first, ProxyContainer second)
        {
            ProxyContainer temp = first;
            first = second;
            second = temp;
        }
    }
}
