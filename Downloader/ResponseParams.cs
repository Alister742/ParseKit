using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Downloader
{
    class ResponseParams : ICloneable
    {
        public bool AllowRedirect { get; set; }
        public int MaxPageSize { get; set; }
        public int DownloadTimeoutMs { get; set; }
        public Encoding Encoding { get; set; }
        public bool ConvertToString { get; set; }


        public ResponseParams()
        {
            MaxPageSize = Config.ResponceSet.MaxPageSize;
            DownloadTimeoutMs = Config.ResponceSet.DownloadTimeoutMs;
            AllowRedirect = Config.ResponceSet.AllowRedirect;
            Encoding = Config.ResponceSet.DefaultEncoding;
            ConvertToString = Config.ResponceSet.ConvertToString;
        }

        #region Члены ICloneable

        public object Clone()
        {
            return new ResponseParams()
                       {
                           AllowRedirect = this.AllowRedirect,
                           ConvertToString = this.ConvertToString,
                           DownloadTimeoutMs = this.DownloadTimeoutMs,
                           Encoding = this.Encoding,
                           MaxPageSize = this.MaxPageSize
                       };
        }

        #endregion
    }
}
