using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core
{
    /// <summary>
    /// Class container for all main constants of project
    /// TODO: Change this for configuring behavior
    /// </summary>
    internal static class Config /* Configuration */
    {
        #region ProxyProvider
        public const int Lh_lives = 100;
        public const int Lh_occupation = 1;
        #endregion

        #region Downloader
        public const int ThreadBoundary = 300;
        public const int ParallelReqCap = 1;
        #endregion

        /* default downloader parameters, all of this used in DownloaderObj constructor */
        #region DownloaderObj
        public const bool NeedString = false;
        public const int Attempts = 4;
        public const int AttemptPause = 5000;
        #endregion

        #region RequestParams
        public static Encoding DefaultEncoding = Encoding.UTF8;
        public const int MaxRedirections = 5;
        public const bool KeepAlive = true;
        public const string Method = "GET";
        public const bool UseRedirect = true;
        //for constructor
        public const string DefaulAgent = UserAgentHeader.Opera;
        public const string DefaultAccept = "text/html, application/xml;q=0.9, application/xhtml+xml, image/png, image/webp, image/jpeg, image/gif, image/x-xbitmap, */*;q=0.1";
        #endregion
    }
}
