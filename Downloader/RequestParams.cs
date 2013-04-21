using System;
using System.Net;

namespace Downloader
{
    public class RequestParamsBase
    {
    }

    public class RequestParams : RequestParamsBase, ICloneable
    {
        public Uri Uri { get; set; }
        public IProxyContainer PrxContainer { get; set; }
        public CookieCollection Cookie { get; set; }
        public byte[] PostData { get; set; }
        //public TimingParams TimingParams { get; set; }
        public DecompressionMethods Decompression { get; set; }
        public int RequestTimeout { get; set; }
        public int ReadWriteTimeout { get; set; }

        //public RequestParams RequestParams { get; set; }

        //public string UserAgent { get; set; }
        //public string Accept { get; set; }
        //defaultHead.Add("Accept-Language:ru-RU,ru;q=0.9,en;q=0.8");
        public WebHeaderCollection Headers { get; set; }
        public bool KeepAlive { get; set; }
        public string Method { get; set; }
        //public string ContentType { get; set; }
        //public int Redirections { get; set; } //[NOT SUPPORT YET]

        public RequestParams()
        {
            Headers = Config.RequestSet.Headers;
            Decompression = Config.RequestSet.Decompression;
            Cookie = new CookieCollection();
            KeepAlive = Config.RequestSet.KeepAlive;
            Method = Config.RequestSet.DefaultMethod;
            RequestTimeout = Config.RequestSet.RequestTimeout;
            ReadWriteTimeout = Config.RequestSet.ReadWriteTimeout;
        }

        #region Члены ICloneable

        /// <summary>
        /// WARRING: some of the reference fields of cloned object is reference assigning
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            RequestParams clone =  new RequestParams()
                       {
                           Uri = new Uri(this.Uri.OriginalString),
                           PrxContainer = this.PrxContainer,                //Reference assigning
                           Cookie = this.Cookie,                            //Reference assigning
                           PostData = this.PostData,
                           Decompression = this.Decompression,
                           RequestTimeout = this.RequestTimeout,
                           Headers =  this.Headers,                         //Reference assigning
                           KeepAlive = this.KeepAlive,
                           Method = this.Method
                       };

            return clone;
        }

        #endregion
    }
}
