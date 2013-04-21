using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.Net.Cache;
using System.IO.Compression;
using System.IO;

namespace Core
{
    public static class UriHandler
    {
        public static Uri CreateUri(string url, string sheme = "http", string host = "")
        {
            Uri uri;
            if (Uri.IsWellFormedUriString(url, UriKind.Relative))
            {
                bool isFixedHost = !string.IsNullOrEmpty(host);
                if (isFixedHost) host = host + "/";

                url = sheme + "://" + (host  + url).Replace("//", "/");
            }
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
                return null;

            if (uri.Scheme == Uri.UriSchemeMailto)
                return null;

            return uri;
        }
    }
}