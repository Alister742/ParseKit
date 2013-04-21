using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Downloader
{
    class _DownloadHandler
    {
        static CookieContainer _sharedCookies = new CookieContainer();          //PUBLIC SHARED COOKIES

        internal void Receive(DownloaderObj obj)
        {
            while (obj.Attempts > 0)
            {
                obj.Attempts--;
                try
                {
                    obj.Request = CreateRequest(obj);
                    if (obj.PostData != null)
                        PostRequestData(obj.Request, obj.PostData);

                    obj.Response = obj.Request.GetResponse() as HttpWebResponse;

                    HandleRedirectAndCookies(obj);

                    if (TryReceiveData(obj))
                        break;
                }
                catch (WebException e)
                {
                    GlobalLog.Err(e, "Host: " + obj.Uri.Host);
                    HandleWebState(e, obj);
                }
                finally
                {
                    if (obj.Response != null)
                        obj.Response.Close();
                }
            }
        }

        internal void BeginReceive(DownloaderObj obj)
        {
            if (obj.Attempts > 0)
            {
                obj.Attempts--;
                try
                {
                    obj.Request = CreateRequest(obj);
                    if (obj.PostData != null)
                        PostRequestData(obj.Request, obj.PostData);

                    //Console.WriteLine("Send request to -- {0}", obj.Uri.OriginalString);
                    obj.Request.BeginGetResponse(EndReceive, obj);
                }
                catch (WebException e)
                {
                    GlobalLog.Err(e, "Host: " + obj.Uri.Host);
                    HandleWebState(e, obj);
                    RetryOrCallback(obj);
                }
                catch (Exception e)
                {
                    GlobalLog.Err(e, "Host: " + obj.Uri.Host);
                    RetryOrCallback(obj);
                }
            }
        }

        internal void EndReceive(IAsyncResult ar)
        {
            if (!ExecutionContext.IsFlowSuppressed())
                ExecutionContext.SuppressFlow();

            DownloaderObj obj = ar.AsyncState as DownloaderObj;

            try
            {
#if DEBUG
                GlobalLog.Write("Get responce from " + obj.Uri.Host);
#endif
                obj.Response = obj.Request.EndGetResponse(ar) as HttpWebResponse;

                HandleRedirectAndCookies(obj);

                if (TryReceiveData(obj))
                {
                    CallbackAndContinue(obj);
                    return;
                }
#if DEBUG
                GlobalLog.Write("CANT Downloaded data from " + obj.Uri.Host);
#endif
            }
            catch (WebException e)
            {
                GlobalLog.Err(e, "Host: " + obj.Uri.Host);
                HandleWebState(e, obj);
            }
            catch (Exception e)
            {
                GlobalLog.Err(e, "Host: " + obj.Uri.Host);
            }
            finally
            {
                if (obj.Response != null)
                    obj.Response.Close();

            }
            RetryOrCallback(obj);
        }

        //Method need some refactoring
        private bool TryReceiveData(DownloaderObj obj)
        {
            byte[] data = ReadResponseStream(obj.Response, obj.TimingParams);
            obj.State = new DownloadStateProvider().GetWebState(obj.Request, obj.Response);

            if (data != null)
            {
                if (obj.NeedString)
                    obj.DataStr = obj.RequestParam.Encoding.GetString(data);
                else
                    obj.Data = data;
                return true;
            }
            else
            {
                return obj.State == DownloadState.Success_2xx || obj.State == DownloadState.Info_1xx;
            }
        }

        void HandleRedirectAndCookies(DownloaderObj obj)
        {
            if (obj.Response == null)
                return;

            Uri redirect = null;
            HandleCookies(obj);

            while (obj.RequestParam.UseRedirect && obj.RequestParam.Redirections > 0 && CheckRedirect(obj, ref redirect))
            {
                obj.RequestParam.Redirections--;
                obj.Response = GetResponse(obj);
                HandleCookies(obj);
            }
        }

        void PostRequestData(HttpWebRequest request, byte[] data)
        {
            if (data == null)
                return;

            try
            {
                request.ContentLength = data.Length;
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                }
                return;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private HttpWebResponse GetResponse(DownloaderObj obj)
        {
            HttpWebRequest request = CreateRequest(obj);
            //ADD POST DATA HANDLING ??
            return request.GetResponse() as HttpWebResponse;
        }

        byte[] ReadResponseStream(HttpWebResponse response, TimingParams param)
        {
            DateTime start = DateTime.Now;
            bool useDownlTimeout = param.DownloadTimeout == Timeout.Infinite ? false : true;
            byte[] data = null;
            byte[] buffer = new byte[8 * 1024];

            try
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        int readed;
                        while ((readed = stream.Read(buffer, 0, buffer.Length)) > 0 && ms.Length < param.MaxPageSize)
                        {
                            ms.Write(buffer, 0, readed);
                            if (useDownlTimeout && (DateTime.Now - start).TotalMilliseconds > param.DownloadTimeout)
                                break;
                        }
                        data = ms.GetBuffer();
                    }
                }
                return data;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        void HandleWebState(WebException e, DownloaderObj obj)
        {
            obj.State = new DownloadStateProvider().GetWebState(obj.Request, obj.Response);

            if (obj.State == DownloadState.BadAddress ||
                obj.State == DownloadState.DocumentUnavailable ||
                obj.State == DownloadState.ProxyError)
            {
                obj.Attempts = 0;
            }

            if (obj.State == DownloadState.ServiceUnavailable && obj.Attempts > 0)
            {
                Thread.Sleep(Rnd.Next(obj.AttemptPause / 2, (int)(obj.AttemptPause * 1.5)));
            }
        }

        void RetryOrCallback(DownloaderObj obj)
        {
            if (obj.Attempts > 0)
            {
                Thread.Sleep(Rnd.Next(obj.AttemptPause / 2, (int)(obj.AttemptPause * 1.5)));
                BeginReceive(obj);
            }
            else
            {
                CallbackAndContinue(obj);
            }
        }

        void CallbackAndContinue(DownloaderObj obj)
        {
            if (obj.CallBack != null)
            {
                try
                {
                    obj.CallBack.Invoke(obj);
                }
                catch (Exception e)
                {
                    GlobalLog.Err(e, "CallBack err");
                }
            }
            Downloader.ProcessNext(obj.Uri);
        }

        public bool HaveResponce(DownloaderObj obj)
        {
            while (obj.Attempts > 0)
            {
                obj.Attempts--;
                try
                {
                    obj.Request = CreateRequest(obj);
                    bool haveResp = obj.Request.HaveResponse;
                    obj.Request.Abort();
                    return haveResp;
                }
                catch (WebException e)
                {
                    GlobalLog.Err(e, "Host: " + obj.Uri.Host);
                    HandleWebState(e, obj);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return false;
        }

        #region Cookies
        public CookieCollection GetCookies(DownloaderObj obj)
        {
            while (obj.Attempts > 0)
            {
                obj.Attempts--;
                try
                {
                    obj.Request = CreateRequest(obj);
                    obj.Response = obj.Request.GetResponse() as HttpWebResponse;
                    if (obj.Request.HaveResponse && obj.Response.Cookies != null)
                        return obj.Response.Cookies;
                }
                catch (WebException e)
                {
                    GlobalLog.Err(e, "Host: " + obj.Uri.Host);
                    HandleWebState(e, obj);
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    if (obj.Request != null)
                        obj.Request.Abort();
                    if (obj.Response != null)
                        obj.Response.Close();
                }
            }
            return null;
        }

        void HandleCookies(DownloaderObj obj)
        {
            if (obj.Response == null)
                return;

            //obj.Response.Cookies = FishHeadersCookies(obj.Response.Headers);

            //if (obj.CookieOptions.HasFlag(CookieOptions.TakeAway))
            //{
            //    obj.Cookie = obj.Response.Cookies;
            //}
            if (obj.CookieOptions.HasFlag(CookieOptions.SaveShared))
            {
                SaveCookies(obj.Uri, obj.Response.Cookies);
            }
            if (obj.CookieOptions.HasFlag(CookieOptions.Take))
            {
                obj.Cookie = obj.Response.Cookies;
            }
        }

        public static void SaveCookies(Uri uri, CookieCollection cookies)
        {
            if (cookies == null)
                return;

            Uri host = UriHandler.CreateUri(string.Empty, uri.Scheme, uri.Host);

            if (uri != null)
                _sharedCookies.Add(host, cookies);
        }

        public static void ClearCookies()
        {
            _sharedCookies = new CookieContainer();
        }

        CookieContainer BuildCookieContainer(CookieOptions cookieOptions, CookieCollection insertCookie)
        {
            if (cookieOptions.HasFlag(CookieOptions.NoCookies))
                return null;

            CookieContainer container = null;
            if (cookieOptions.HasFlag(CookieOptions.UseShared))
            {
                container = _sharedCookies;
            }
            else
            {
                container = new CookieContainer();
            }
            if (insertCookie != null)
                container.Add(insertCookie);

            return container;
        }

        private CookieCollection FishHeadersCookies(WebHeaderCollection headers)
        {
            CookieCollection cookies = new CookieCollection();
            foreach (string header in headers)
            {
                if (header.Contains("SetCookie") || header.Contains("Set-Cookie"))
                {
                    string[] cookieParam = headers[header].Split(';');
                    for (int i = 0; i < cookieParam.Length; i++)
                    {
                        //cookieParam[i]
                    }
                }
            }
            return cookies;
        }
        #endregion

        bool CheckRedirect(DownloaderObj obj, ref Uri redirUri)
        {
            if (obj.Response.Headers == null)
                return false;

            string location = obj.Response.Headers[HttpResponseHeader.Location];

            if (location != null)
            {
                string host = obj.Response.ResponseUri.Host;
                string sheme = obj.Response.ResponseUri.Scheme;

                redirUri = UriHandler.CreateUri(location, sheme, host);
                return true;
            }
            return false;
        }

        HttpWebRequest CreateRequest(DownloaderObj obj)
        {
            return CreateRequest(obj.Uri, obj.CookieOptions, obj.TimingParams, obj.RequestParam, obj.Proxy, obj.Cookie);
        }
        HttpWebRequest CreateRequest(Uri uri, CookieOptions cookieOptions, TimingParams param, RequestParams requestParam, RatedProxy proxy, CookieCollection insertCookie)
        {
            HttpWebRequest request = HttpWebRequest.Create(uri) as HttpWebRequest;

            request.Headers = requestParam.Headers;
            request.UserAgent = requestParam.UserAgent;
            request.Host = uri.Host;
            request.Accept = requestParam.Accept;
            request.ContentType = requestParam.ContentType;
            //request.Referer = "http://www.google.com/";
            request.AllowAutoRedirect = requestParam.UseRedirect;
            request.KeepAlive = requestParam.KeepAlive;
            request.AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip;
            request.Proxy = proxy;
            request.Timeout = param.RequestTimeout;
            request.ReadWriteTimeout = param.GetStreamTimeout;
            request.CookieContainer = BuildCookieContainer(cookieOptions, insertCookie);
            request.Method = requestParam.Method;

            return request;
        }
    }
}
