using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Core;

namespace Downloader
{
    public class HttpSession
    {
        public int RetryInterval { get; set; }
        public int Attempts { get; private set; }
        public object Container { get; set; }
        public bool CollectCookie { get; set; }
        public CookieContainer AllReceivedCookies { get; set; }

        private readonly Action<HttpSession> _callBack;

        public HttpSession(Action<HttpSession> callBack)
        {
            _callBack = callBack;
            RetryInterval = Config.HttpSessionSet.RetryInterval;
            Attempts = Config.HttpSessionSet.Attempts;
            CollectCookie= Config.HttpSessionSet.CollectCookie;
            AllReceivedCookies = new CookieContainer();  
        }

        public void BeginSession(RequestParams reqPrmsPattern, ResponseParams resPrmsPattern)
        {
            StartNewTransaction(reqPrmsPattern, resPrmsPattern);
        }

        private void StartNewTransaction(RequestParams reqPrmsPattern, ResponseParams resPrmsPattern)
        {
            Transaction tr = new Transaction(reqPrmsPattern, resPrmsPattern, TransactionCallback);
            Transactions.Add(tr);

            if (Attempts > 0)
            {
                tr.Begin();
            }
            else
            {
                SessionCallback();
            }
        }

        public void Abort()
        {
            Transactions.Last().Abort();
        }

        private void UseOneAttempt()
        {
            Attempts--;
        }

        private void SessionCallback()
        {
            try
            {
                _callBack.Invoke(this);
            }
            catch (Exception e)
            {
                GlobalLog.Err(e, "Error while callback in HttpSession, please add catch below");
            }
        }

        private void TransactionCallback(Transaction trans)
        {
            switch (trans.Result)
            {
                case  TransactionResult.Fail:
                    {
                        var reqPrms = trans.RequestParams.Clone() as RequestParams;
                        var resPrms = trans.ResponseParams.Clone() as ResponseParams;

                        UseOneAttempt();
                        StartNewTransaction(reqPrms, resPrms);
                    }
                    break;
                case TransactionResult.Success:
                    {
                        if (CollectCookie)
                        {
                            SaveCookies(trans.Responce.ResponseUri, trans.Responce.Cookies);
                        }

                        if (trans.ResponseParams.AllowRedirect)
                        {
                            Uri redirect = null;
                            if (CheckRedirect(trans.Responce, ref redirect))
                            {
                                RedirectWithCookie(trans, redirect);
                            }
                        }
                    }
                    break;
                default:
                    SessionCallback();
                    break;
            }
        }

        private void RedirectWithCookie(Transaction trans, Uri redirect)
        {
            var reqPrms = trans.RequestParams.Clone() as RequestParams;
            reqPrms.Uri = redirect;
            reqPrms.Cookie = trans.Responce.Cookies;

            var resPrms = trans.ResponseParams.Clone() as ResponseParams;
            
            StartNewTransaction(reqPrms, resPrms);
        }

        bool CheckRedirect(HttpWebResponse response, ref Uri redirUri)
        {
            if (response.Headers == null)
                return false;

            var location = response.Headers[HttpResponseHeader.Location];

            if (location != null)
            {
                var host = response.ResponseUri.Host;
                var sheme = response.ResponseUri.Scheme;

                redirUri = UriHandler.CreateUri(location, sheme, host);
                return true;
            }
            return false;
        }

        private void SaveCookies(Uri uri, CookieCollection cookies)
        {
            if (CollectCookie)
            {
                if (cookies == null || uri == null)
                    return;

                var host = UriHandler.CreateUri(string.Empty, uri.Scheme, uri.Host);

                if (host != null)
                    AllReceivedCookies.Add(host, cookies);
            }
        }

        public List<Transaction> Transactions { get; private set; }
    }
}
