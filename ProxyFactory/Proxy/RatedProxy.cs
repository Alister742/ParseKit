using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;


namespace ProxyFactory
{
    public enum AnonymousLevel
    {
        NotAnonymous = 0,
        Anonymous = 1,
        HightAnonymous = 2,
    }

    public class RatedProxy : WebProxy, IProxyContainer
    {
        object _downloadsSync = new object();
        public const int DefaultVal = -1;

        public RatedProxy Proxy { get { return this; } }

        public int AvgLatency
        {
            get { return _avglatency; }
            set { _avglatency = CalcNewValue(value, _avglatency, CheckTimes); }
        }
        public AnonymousLevel AnonymousLevel;
        public double SitesRate
        {
            get { return _avgSitesRate; }
            set { _avgSitesRate = CalcNewValue(value, _avgSitesRate, CheckTimes); }
        }
        public double GoogleRate
        {
            get { return _googleRate; }
            set { _googleRate = CalcNewValue(value, _googleRate, GoogleChecked); }
        }
        public double YaRate
        {
            get { return _yaRate; }
            set { _yaRate = CalcNewValue(value, _yaRate, YaChecked); }
        }
        public double MultidownloadRate
        {
            get { return _avgMultidownloadRate; }
            set 
            {
                lock (_downloadsSync) 
                    _avgMultidownloadRate = CalcNewValue(value, _avgMultidownloadRate, CheckTimes); 
            }
        }
        public double RBLBanRate { get; set; }
        public double AvgSpeed
        {
            get { return _avgSpeed; }
            set { _avgSpeed = CalcNewValue(value, _avgSpeed, CheckTimes); }
        }
        public double SEQuality { get { return (_googleRate + _yaRate) / 2; } }
        public int GoogleChecked { get; set; }
        public int YaChecked { get; set; }
        public int CheckTimes { get; set; }

        int _avglatency;
        double _avgSitesRate;
        double _googleRate;
        double _yaRate;
        double _avgMultidownloadRate;
        double _avgSpeed; //KB-sec

        public RatedProxy(
            string address,
            int checkedTimes = 0,
            double sitesRate = DefaultVal,
            double rblBanRate = DefaultVal,
            AnonymousLevel anonymousLevel = 0,
            int latency = DefaultVal,
            int downloadSpeed = DefaultVal,
            double multiDownloadRate = DefaultVal,
            double yaRate = DefaultVal,
            int yaChecked = 0,
            double googlRate = DefaultVal,
            int googChecked = 0)
        {
            Address = new Uri("http://" + address + "/");
            _avglatency = latency;
            _yaRate = yaRate;
            _googleRate = googlRate;
            CheckTimes = checkedTimes;
            _avgSitesRate = sitesRate;
            GoogleChecked = googChecked;
            YaChecked = yaChecked;
            _avgMultidownloadRate = multiDownloadRate;
            RBLBanRate = rblBanRate;
            AnonymousLevel = anonymousLevel;
            _avgSpeed = downloadSpeed;
        }

        double CalcNewValue(double incVal, double oldVal, int counter)
        {
            return oldVal == DefaultVal ? incVal : (oldVal * counter + incVal) / (counter + 1);
        }

        int CalcNewValue(int incVal, int oldVal, int counter)
        {
            return (int)CalcNewValue((double)incVal, (double)oldVal, counter);
        }
    }
}
