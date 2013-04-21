using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Downloader
{
    static class Config
    {
        public static class UserAgentHeaders
        {
            public const string Opera       = "User-Agent:Opera/9.80 (Windows NT 5.1; U; ru) Presto/2.10.229 Version/11.61";
            public const string Chrome      = "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.22 (KHTML, like Gecko) Chrome/25.0.1364.152 Safari/537.22";
            public const string Firefox     = "Mozilla/5.0 (Windows NT 5.1; rv:18.0) Gecko/20100101 Firefox/18.0";
            public const string IE7         = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 1.1.4322; .NET4.0C; .NET4.0E; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
        }

        public static class HttpSessionSet
        {
            public const int RetryInterval = 1000;
            public const int Attempts = 3;
            public const bool CollectCookie = true;
        }
        public static class ResponceSet
        {
            public const int MaxPageSize = 512 * 1024;           // (512kB)
            public const int DownloadTimeoutMs = 60 * 1000;      // (60sec)
            public const bool AllowRedirect = true;
            public static readonly Encoding DefaultEncoding = Encoding.UTF8;
            public const bool ConvertToString = true;
        }

        public static class RequestSet
        {
            public static WebHeaderCollection Headers
            {
                get
                {
                    var whc =  new WebHeaderCollection();

                    whc.Add("User-Agent", UserAgentHeaders.Opera);
                    whc.Add("Accept", "text/html, application/xml;q=0.9, application/xhtml+xml, image/png, image/webp, image/jpeg, image/gif, image/x-xbitmap, */*;q=0.1");
                    whc.Add("Accept-Language", "ru-RU,ru;q=0.9,en;q=0.8");
                    //whc.Add("ContentType", "");
                    return whc;
                }
            }

            public const bool KeepAlive = true;
            public const int RequestTimeout = 15 * 1000;            // (15sec)
            public const string DefaultMethod = RequestMethods.GET;
            public static int ReadWriteTimeout = 15 * 1000;         // (15sec)

            public const DecompressionMethods Decompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

        }
        public static class DownloaderSet
        {
            public const int ThreadBoundary = 300;
            public const int ParallelReqCap = 1;
        }
    }
}
