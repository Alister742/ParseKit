using System;
using System.IO;
using System.Net;
using System.Threading;
using Core;

namespace Downloader
{
    enum TransactionStage
    {
        Initial,
        CreatingRequest,
        DataPost,
        BeginReceiveResponce,
        EndReceiveResponce,
        ReceivingData,
        Finishing,
    }

    enum TransactionResult
    {
        Initialized,
        Carried,
        Aborted,
        Success,
        Fail,
    }

    class Transaction
    {
        private readonly Action<Transaction> _callBack;
        private DateTime _startDate;

        public Transaction(RequestParams reqPrms, ResponseParams resPrms, Action<Transaction> callBack)
        {
            if (callBack==null)
                throw new ArgumentNullException("Transaction callback can not be null");

            _callBack = callBack;

            Result = TransactionResult.Initialized;
            Stage = TransactionStage.Initial;
        }

        public void Begin()
        {
            _startDate = DateTime.Now;
            Result = TransactionResult.Carried;
            try
            {
                Stage = TransactionStage.CreatingRequest;
                CreateRequest();

                Stage = TransactionStage.DataPost;
                PostData();

                Stage = TransactionStage.BeginReceiveResponce;
                BeginReceiveResponse();
            }
            catch (Exception e)
            {
                Error = e;
                Result = TransactionResult.Fail;
            }
            finally
            {
                FinishTransaction();
                Callback();
            }
        }

        public void Abort()
        {
            if (Request!=null)
            {
                Request.Abort();
                CloseResponce();
                StopTiming();
                Result = TransactionResult.Aborted;
            }
        }

        private void EndProvideTransaction(IAsyncResult ar)
        {
            try
            {
                if (!ExecutionContext.IsFlowSuppressed())
                    ExecutionContext.SuppressFlow();

                Stage = TransactionStage.EndReceiveResponce;
                EndReceiveResponse(ar);

                Stage = TransactionStage.ReceivingData;
                ReceiveData();

                Stage = TransactionStage.Finishing;
            }
            catch (Exception e)
            {
                Error = e;
                Result = TransactionResult.Fail;
            }
            finally
            {
                CloseResponce();
                StopTiming();
                FinishTransaction();
                Callback();
            }
        }

        private void PostData()
        {
            var data = RequestParams.PostData;

            if (data == null)
                return;

            Request.ContentLength = data.Length;
            using (var stream = Request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
        }

        private void CreateRequest()
        {
            Request = HttpWebRequest.Create(RequestParams.Uri) as HttpWebRequest;

            Request.Headers = RequestParams.Headers;
            //request.UserAgent = Params.RequestParam.UserAgent;
            Request.Host = RequestParams.Uri.Host;
            //request.Accept = requestParam.Accept;
            //request.ContentType = requestParam.ContentType;
            //request.Referer = "http://www.google.com/";
            Request.AllowAutoRedirect = false;
            Request.KeepAlive = RequestParams.KeepAlive;
            Request.AutomaticDecompression = RequestParams.Decompression;
            Request.Proxy = RequestParams.PrxContainer as IWebProxy;
            Request.Timeout = RequestParams.RequestTimeout;
            Request.ReadWriteTimeout = RequestParams.ReadWriteTimeout;
            Request.CookieContainer = BuildCookieContainer(RequestParams.Cookie);
            Request.Method = RequestParams.Method;
        }

        private static CookieContainer BuildCookieContainer(CookieCollection insertCookie)
        {
            var container = new CookieContainer();

            if (insertCookie != null)
                container.Add(insertCookie);

            return container;
        }

        private void BeginReceiveResponse()
        {
            Request.BeginGetResponse(EndProvideTransaction, null);
        }

        private void EndReceiveResponse(IAsyncResult ar)
        {
            if (!ExecutionContext.IsFlowSuppressed())
                ExecutionContext.SuppressFlow();

            Responce = Request.EndGetResponse(ar) as HttpWebResponse;

            ResonceStatus = new DownloadStateProvider().GetWebState(Request, Responce);


            //HandleRedirectAndCookies(obj);

            //if (TryReceiveData(obj))
            //{
            //    CallbackAndContinue(obj);
            //    return;
            //}
//#if DEBUG
//                GlobalLog.Write("CANT Downloaded data from " + obj.Uri.Host);
//#endif

            //catch (WebException e)
            //{
            //    GlobalLog.Err(e, "Host: " + obj.Uri.Host);
            //    HandleWebState(e, obj);
            //}
            //catch (Exception e)
            //{
            //    GlobalLog.Err(e, "Host: " + obj.Uri.Host);
            //}
            //finally
            //{
            //    if (obj.Response != null)
            //        obj.Response.Close();

            //}
            //RetryOrCallback(obj);
        }

        private void ReceiveData()
        {
            var start = DateTime.Now;
            BinaryData = ReadResponseStream();
            var end = DateTime.Now;

            ElapsedRead = (int) (end - start).TotalMilliseconds;

            if (BinaryData != null && ResponseParams.ConvertToString)
            {
                if (ResponseParams.Encoding == null)
                {
                    throw new ArgumentException("Encoding can not be null if convert flag set to true");
                }

                StringData = ResponseParams.Encoding.GetString(BinaryData);
            }
        }

        private byte[] ReadResponseStream()
        {
            var start = DateTime.Now;
            var timeout = ResponseParams.DownloadTimeoutMs;
            var maxPageSize = ResponseParams.MaxPageSize;
            var enableTimeout = timeout != Timeout.Infinite;
            var buffer = new byte[8 * 1024];
            try
            {
                byte[] data = null;
                using (var stream = Responce.GetResponseStream())
                using (var ms = new MemoryStream())
                {
                    int readed;
                    while ((readed = stream.Read(buffer, 0, buffer.Length)) > 0 && ms.Length < maxPageSize)
                    {
                        ms.Write(buffer, 0, readed);
                        if (enableTimeout && (DateTime.Now - start).TotalMilliseconds > timeout)
                            break;
                    }
                    data = ms.GetBuffer();
                }
                return data;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void CloseResponce()
        {
            if (Responce!=null)
            {
                Responce.Close();
            }
            //if (Request!=null)
            //{
            //    Request.Abort();
            //}
        }

        private void FinishTransaction()
        {
            StopTiming();

            Result = Error == null ? TransactionResult.Success : TransactionResult.Fail;
        }

        private void StopTiming()
        {
            ElapsedTotal = (int)(DateTime.Now - _startDate).TotalMilliseconds;
        }

        private void Callback()
        {
            try
            {
                _callBack.Invoke(this);
            }
            catch (Exception e)
            {
                GlobalLog.Err(e, "Error while callback in Transaction, please add catch below");
            }
        }

        public HttpWebRequest Request { get; private set; }
        public HttpWebResponse Responce { get; private set; }

        public int ElapsedTotal { get; private set; }
        public int ElapsedRead { get; private set; }
        public byte[] BinaryData { get; private set; }
        public string StringData { get; private set; }

        public RequestParams RequestParams { get; private set; }
        public ResponseParams ResponseParams { get; private set; }

        public TransactionStage Stage { get; private set; }
        public TransactionResult Result { get; private set; }
        public Exception Error { get; private set; }
        public ResponseState ResonceStatus { get; private set; }
    }
}
