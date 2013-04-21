using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.Concurrent;
using System.Threading;
using System.Text.RegularExpressions;





namespace ParseKit.Data
{
    public class DevourTarget
    {
        public int Attempts { get; set; }
        public int Lifes { get; set; }
        public Uri Uri { get { return _uri; } }
        public IPageReader Reader { get { return _reader; } }
        public Object Obj { get; set; }

        IPageReader _reader;
        Uri _uri;

        public DevourTarget(int attempts, Uri uri, IPageReader reader, int lifes = 1)
        {
            if (uri == null || reader == null)
                throw new ArgumentException("Bad arguments");

            Attempts = attempts;
            Lifes = lifes;
            _uri = uri;
            _reader = reader;
        }
    }

    /// <summary>
    /// Download site pages throught proxies
    /// </summary>
    public class Devourer 
    {
        object sync = new object();

        public delegate void WorkCompleteDel();
        public delegate void WorkStoppedeDel(List<DevourTarget> leftTargets);
        public delegate void OnProgressDel(int completed, int processing);
        public delegate void OnNewTargetsDel(int count);
        public delegate void OnTargetErrorDel(DevourTarget brokenTarget);
        public event WorkCompleteDel OnWorkComplete;
        public event WorkStoppedeDel OnWorkBroken;
        public event OnProgressDel OnDownloadProgress;
        public event OnProgressDel OnReadProgress;
        public event OnNewTargetsDel OnNewTargets;
        public event OnNewTargetsDel OnTargetsQueued;
        public event OnTargetErrorDel OnTargetError;


        /////// <summary>
        /////// Count of attempt for download page, 
        /////// when attempts ended and page still no downloaded his live decrease by 1
        /////// </summary>
        ////public int Attempts { get { return _attempts; } set { _attempts = value; } }
        ///// <summary>
        ///// Count of lives have any page Uri,
        ///// when this will 0 Uri will be added in FaultTargets list
        ///// </summary>
        ////public int TargetLives { get { return _maxTargetLifes; } set { _maxTargetLifes = value; } }
        public RequestParams RequestParams { get; set; }
        public CookieOptions CookieOptions { get; set; }
        public TimingParams TimingParams { get; set; }
        /// <summary>
        /// If set as true all success downloaded Uri's will be added in SuccessTargets list
        /// </summary>
        public bool SaveSuccessTargets { get { return _saveSuccessTargets; } set { _saveSuccessTargets = value; } }
        public List<Uri> SuccessTargets { get { return _successTargets; } }
        public List<DevourTarget> FaultTargets { get { lock (sync) return _faultTargets; } }
        public HashSet<string> AllTargets { get { return _allTargets; } }
        public bool PreventSimilarTargetDownload { get; set; }

        //int _attempts;
        List<DevourTarget> _faultTargets;
        List<Uri> _successTargets;
        HashSet<string> _allTargets;
        //int _maxTargetLifes;
        bool _saveSuccessTargets;
        int _objQueued;
        int _objDownloaded;
        int _readsQueued;
        int _readsComplete;

        IProxyProvider _proxies;
        IPageValidator _validator;

        ConcurrentQueue<DevourTarget> _targets;           /* KEY -- faultCount, VALUE -- targetUri */
        //RequestParams _params;

        public Devourer(DevourTarget initialTarget, IProxyProvider proxies, IPageValidator validator = null)
            : this(new List<DevourTarget>(new DevourTarget[] { initialTarget }), proxies, validator)
        {
        }


        public Devourer(List<DevourTarget> initialTargets, IProxyProvider proxies, IPageValidator validator = null)
        {
            if (initialTargets == null || initialTargets.Count == 0 /*|| reader == null */ || proxies == null)
                throw new ArgumentException("Bad arguments");

            PreventSimilarTargetDownload = false;
            CookieOptions = CookieOptions.Empty;

            _proxies = proxies;
            _validator = validator;

            _successTargets = new List<Uri>();
            _faultTargets = new List<DevourTarget>();
            _allTargets = new HashSet<string>();

            InitializeStartTargets(initialTargets);

            SubscribeProxyProvider();
        }

        void InitializeStartTargets(List<DevourTarget> targets)
        {
            _targets = new ConcurrentQueue<DevourTarget>();

            targets.ForEach(t =>
            {
                SubscribeDevourTarget(t);
                _targets.Enqueue(t);
            });
        }

        void _proxies_OnProxiesResulted(object sender, EventArgs e)
        {
            UnsubscribeProxyProvider();

            if (OnWorkBroken != null)
            {
                OnWorkBroken(_targets.ToList());
            }
        }

        void SubscribeProxyProvider()
        {
            _proxies.OnProxyFreed += new FreeProxyDel(_proxies_OnProxyFreed);
            _proxies.OnProxiesResulted += new EventHandler(_proxies_OnProxiesResulted);
        }
        void UnsubscribeProxyProvider()
        {
            _proxies.OnProxyFreed -= new FreeProxyDel(_proxies_OnProxyFreed);
            _proxies.OnProxiesResulted -= new EventHandler(_proxies_OnProxiesResulted);
        }
        void SubscribeDevourTarget(DevourTarget target)
        {
            target.Reader.OnNewTargets += new NewTargetsDel(_reader_OnNewTargets);
            target.Reader.OnReadComplete += new ReadCompleteDel(MoveReadComplete);
        }
        void UnsubscribeDevourTarget(DevourTarget target)
        {
            target.Reader.OnNewTargets -= new NewTargetsDel(_reader_OnNewTargets);
            target.Reader.OnReadComplete -= new ReadCompleteDel(MoveReadComplete);
        }

        #region Recrut Mechanizm
        void _reader_OnNewTargets(List<DevourTarget> targets)
        {
            targets.ForEach((t) => 
            {
                SubscribeDevourTarget(t);

                if (PreventSimilarTargetDownload)
                {
                    if (!_allTargets.Contains(t.Uri.OriginalString))
                    {
                        if (!TryDevourTarget(t))
                        {
                            _targets.Enqueue(t);
                        }
                    }
                }
                else
                {
                    if (!TryDevourTarget(t))
                    {
                        _targets.Enqueue(t);
                    }
                }
                _allTargets.Add(t.Uri.OriginalString);
            });
        }

        private bool TryDevourTarget(DevourTarget target)
        {
            ProxyContainer proxy;
            if (_proxies.TryGet(out proxy))
            {
                DevourOne(proxy, target);
                return true;
            }
            return false;
        }

        private void _proxies_OnProxyFreed(ProxyContainer proxy)
        {
            DevourTarget target;
            if (_targets.TryDequeue(out target))
            {
                DevourOne(proxy, target);
            }
        }
        #endregion

        public void Start()
        {
            if (!(_targets.Count > 0 && _proxies.SlotsAvailable > 0))
                throw new ArgumentException("Nothing to devour or no proxy slots");

            DevourTarget target;
            ProxyContainer proxy;
            while (_targets.TryDequeue(out target) && _proxies.TryGet(out proxy))
            {
                DevourOne(proxy, target);
            }
        }

#if DEBUG
        int __queuedObjects = 0; 
#endif

        private void DevourOne(ProxyContainer proxyCont, DevourTarget target)
        {
            DownloaderObj obj = new DownloaderObj(target.Uri, DevourCallback, true, proxyCont, this.CookieOptions, target.Attempts, target, null, false, 1000, this.TimingParams, this.RequestParams);
            Downloader.Queue(obj);
#if DEBUG
            Interlocked.Increment(ref __queuedObjects);
            GlobalLog.Write("__queuedObjects: {0}", __queuedObjects);
#endif
            MoveObjQueued();
        }

        private void DevourCallback(DownloaderObj obj)
        {
            if (obj.DataStr != null)
            {
                if (_validator != null)
                {
                    if (_validator.Validate(obj.DataStr))
                        HandleSuccessDownload(obj);
                    else
                        HandleBadDownload(obj);
                }
                else
                {
                    HandleSuccessDownload(obj);
                }
            }
            else
            {
                HandleBadDownload(obj);
            }

            MoveObjComplete();
        }

        private void HandleSuccessDownload(DownloaderObj obj)
        {
            _proxies.Release(obj.PrxContainer as ProxyContainer, true);

            DevourTarget target = obj.Arg as DevourTarget;

            //MoveReadQueue();
            try
            {
                MoveReadQueue();
                target.Reader.ReadData(obj.DataStr, target);
                MoveReadComplete(target);
            }
            catch(Exception ex)
            {
                GlobalLog.Err(ex, "error while reading data in devourer");
            }

            AddSuccessTarget(obj.Uri);
        }

        private void HandleBadDownload(DownloaderObj obj)
        {
            DevourTarget target = obj.Arg as DevourTarget;

            if (obj.State == HttpDownloadResult.BadAddress ||
                obj.State == HttpDownloadResult.DocumentUnavailable)
            {
                AddFaultTarget(target);                                             //Document extremely guilty
                _proxies.Release(obj.PrxContainer as ProxyContainer, false);      //Proxy simply had a bad day
            }
            else
            {
                if (obj.State == HttpDownloadResult.ProxyError)
                {
                    _proxies.Fire(obj.PrxContainer as ProxyContainer);            //Proxy extremely guilty
                    _targets.Enqueue(target);                                       //Document not in the business
                }
                else
                {
                    if (--target.Lifes <= 0)                                        
                    {
                        AddFaultTarget(target);                                     //Sth going wrong
                    }
                    _proxies.Release(obj.PrxContainer as ProxyContainer, false);  //Proxy simply had a bad day
                }
            }
        }

        private void AddFaultTarget(DevourTarget target)
        {
            lock (sync)
            {
                _faultTargets.Add(target);
            }
            if (OnTargetError!=null)
            {
                OnTargetError(target);
            }
        }

        private void AddSuccessTarget(Uri target)
        {
            if (_saveSuccessTargets)
            {
                _successTargets.Add(target);
            }
        }

        #region Notifications

        private void MoveObjQueued()
        {
            Interlocked.Increment(ref _objQueued);
            if (OnDownloadProgress != null)
            {
                OnDownloadProgress(_objDownloaded, _objQueued);
            }
        }
        private void MoveObjComplete()
        {
            Interlocked.Increment(ref _objDownloaded);
            if (OnDownloadProgress != null)
            {
                OnDownloadProgress(_objDownloaded, _objQueued);
            }

#if DEBUG
            GlobalLog.Write("Downloader.Processing: {0}, Queued: {1}", Downloader.Processing, Downloader.Queued);
#endif

            CheckWorkCompletion();
        }
        private void MoveReadQueue()
        {
            Interlocked.Increment(ref _readsQueued);
            if (OnReadProgress != null)
            {
                OnReadProgress(_readsComplete, _readsQueued);
            }
        }
        private void MoveReadComplete(DevourTarget target)
        {
            Interlocked.Increment(ref _readsComplete);
            if (OnReadProgress!=null)
	        {
                OnReadProgress(_readsComplete, _readsQueued);
	        }

#if DEBUG
            GlobalLog.Write("Downloader.Processing: {0}, Queued: {1}", Downloader.Processing, Downloader.Queued);
#endif
            UnsubscribeDevourTarget(target);

            CheckWorkCompletion();
        }
        void CheckWorkCompletion()
        {
            if (_objDownloaded == _objQueued && _targets.Count == 0)// && _readsQueued == _readsComplete)
            {
                if (OnWorkComplete != null)
                {
                    OnWorkComplete();
                }
                UnsubscribeProxyProvider();
            }
        }

        #endregion
    }
}
