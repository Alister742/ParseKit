using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;

namespace Downloader
{
    public enum ResponseState
    {
        /// <summary>
        /// Informational 1xx codes
        /// </summary>
        Info_1xx                = 100,
        /// <summary>
        /// 2xx status codes
        /// </summary>
        Success_2xx             = 200,
        /// <summary>
        /// Responce have specific redirect code and potentially 'Location' header
        /// </summary>
        Redirection_3xx         = 300,
        /// <summary>
        /// HAVE responce but no download because server overload or sth like == connected but no answer was given
        /// </summary>
        ServiceUnavailable      = 1,        
        /// <summary>
        /// Can't connect to remote adress
        /// </summary>
        BadAddress              = 2,       
        /// <summary>
        /// HTTP protocol bad 4xx-5xx statuses
        /// </summary>
        HostHandleProblem       = 3,        
        /// <summary>
        /// Means that server can't give content ever because it's unavailable or retard
        /// </summary>
        DocumentUnavailable     = 4, 
        /// <summary>
        /// POST or GET method request was bad handled
        /// </summary>
        MethodError             = 405,
        /// <summary>
        /// It means some errors with tcp/ip connection
        /// </summary>
        ConnectionProblem       = 5,       
        /// <summary>
        /// Proxy error, timeout or connection stuffs
        /// </summary>
        ProxyError              = 6,        
        /// <summary>
        /// All other errors that was recognized by .net request/responce classes
        /// </summary>
        UnknownSpecified        = 7,        
        /// <summary>
        /// Fully Unknown error
        /// </summary>
        Unknown                 = 8,     
        /// <summary>
        /// Request was awesome lucky
        /// </summary>
        Success                 = 9,
    }

    class DownloadStateProvider
    {

            //if (request != null)
            //{
            //    if (request.HaveResponse && responce != null)
            //    {
            //        return HandleHttpCode(responce.StatusCode);
            //    }
            //    else
            //    {
            //        return DownloadState.ConnectionProblem;
            //    }
            //}
            //else
            //{
            //    return DownloadState.Unknown;
            //}


        public ResponseState GetWebState(HttpWebRequest request, HttpWebResponse responce)
        {
            if (request != null)
            {
                if (request.HaveResponse && responce != null)
                {
                    return HandleHttpCode(responce.StatusCode);
                }
                else
                {
                    return ResponseState.ConnectionProblem;
                }
            }
            else
            {
                return ResponseState.Unknown;
            }
        }

        public ResponseState GetWebState(WebException e, DownloaderObj obj)
        {
            if (e.Response == null)
            {
                return HandleWebExcStatus(e.Status);
            }
            else
            {
                return HandleHttpCode((e.Response as HttpWebResponse).StatusCode);
            }
        }

        private static ResponseState HandleHttpCode(HttpStatusCode status)
        {
            int c = (int)status;

            switch (c)
            {
                case 401:
                case 402:
                case 403:
                case 404:
                case 409: 
                case 501: 
                case 505:
                    return ResponseState.DocumentUnavailable;
                case 407:
                case 502:
                case 504:
                    //GatewayTimeout, BadGateway, ProxyAuthenticationRequired
                    return ResponseState.ProxyError;
                case 408:
                case 503:
                    //ServiceUnavailable, RequestTimeout
                    return ResponseState.ServiceUnavailable;
                case 400:
                case 500:
                    //InternalServerError, BadRequest
                    return ResponseState.HostHandleProblem;
                case 414:
                    //RequestUriTooLong
                    return ResponseState.BadAddress;
                case 406:
                    //NotAcceptable
                    return ResponseState.Success;
                case 405:
                    //MethodNotAllowed
                    return ResponseState.MethodError;
                default:
                    break;
            }

            if (c >= 300 && c < 400)
                return ResponseState.Redirection_3xx;

            if (c >= 200 && c < 300)
                return ResponseState.Success_2xx;

            if (c >= 100 && c < 200)

                return ResponseState.Info_1xx;

            return ResponseState.UnknownSpecified;   
        }

        private static ResponseState HandleWebExcStatus(WebExceptionStatus status)
        {
            int c = (int)status;

            switch (c)
            {
                case 0:
                    //Success
                    return ResponseState.Success;
                case 14:
                    //Timeout
                    return ResponseState.ServiceUnavailable;
                case 12:
                case 10:
                case 8:
                case 5:
                case 2:
                case 11:
                    //KeepAliveFailure, SecureChannelFailure, ConnectionClosed, PipelineFailure, ConnectFailure, ServerProtocolViolation
                    return ResponseState.ConnectionProblem;
                case 7:
                case 3:
                    //ProtocolError, ReceiveFailure
                    return ResponseState.HostHandleProblem;
                case 20:
                case 15:
                    //RequestProhibitedByProxy, ProxyNameResolutionFailure
                    return ResponseState.ProxyError;
                case 1:
                    //NameResolutionFailure
                    return ResponseState.BadAddress;
                case 4:
                case 17:
                    //SendFailure, MessageLengthLimitExceeded
                    return ResponseState.MethodError;
                case 9:
                    //TrustFailure
                    return ResponseState.DocumentUnavailable;

                default:
                    //UnknownError, RequestCanceled, RequestProhibitedByCachePolicy, CacheEntryNotFound
                    return ResponseState.Unknown;
            }
        }
    }
}
