#define DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using Core;

namespace CapchaResolver
{
    //site      antigate.com
    //login     Aae
    //password  12345
    //mail      Aae@asdasd.ru
    //key       da248030640620a5489c9b8d9475f507

    /// <summary>
    /// Container for imag type names
    /// </summary>
    public class ImgType
    {
        string _typeStr;
        private ImgType(string type)
        {
            _typeStr = type;
        }

        public static ImgType Jpg
        {
            get { return new ImgType("image/pjpeg"); }
        }
        public static ImgType Bmp
        {
            get { return new ImgType("image/bmp"); }
        }
        public static ImgType Gif
        {
            get { return new ImgType("image/gif"); }
        }
        public static ImgType Pgn
        {
            get { return new ImgType("image/png"); }
        }

        public override string ToString()
        {
            return _typeStr;
        }
    }

    public enum ErrorState
    {
        OkWithNoID      = 1,
        OkWithNoText    = 2,
        BadImageFormat  = 3,
        BadImageSize    = 4,
        BadUserKey      = 5,
        NoMoney         = 6,
        NoSlots         = 7,
        IpBanned        = 8,
        ConnectProblem  = 9,
        BadCapchaID     = 10,
    }

    /// <summary>
    /// Responce status values
    /// </summary>
    internal class State
    {
        public const string OK                              = "OK";
        public const string CAPCHA_NOT_READY                = "CAPCHA_NOT_READY";
        public const string ERROR_KEY_DOES_NOT_EXIST        = "ERROR_KEY_DOES_NOT_EXIST";
        public const string ERROR_WRONG_ID_FORMAT           = "ERROR_WRONG_ID_FORMAT";
        public const string ERROR_WRONG_USER_KEY            = "ERROR_WRONG_USER_KEY";
        public const string ERROR_ZERO_BALANCE              = "ERROR_ZERO_BALANCE";
        public const string ERROR_NO_SLOT_AVAILABLE         = "ERROR_NO_SLOT_AVAILABLE";
        public const string ERROR_ZERO_CAPTCHA_FILESIZE     = "ERROR_ZERO_CAPTCHA_FILESIZE";
        public const string ERROR_TOO_BIG_CAPTCHA_FILESIZE  = "ERROR_TOO_BIG_CAPTCHA_FILESIZE";
        public const string ERROR_IP_NOT_ALLOWED            = "ERROR_IP_NOT_ALLOWED";
    }

    /// <summary>
    /// Container for posting recognize params
    /// </summary>
    public class RecognizeParams
    {
        public List<KeyValuePair<string, string>> Params { get; private set; }

        /// <param name="phrase">Picture has more than 1 word</param>
        /// <param name="regsense">Word is register sensitive</param>
        /// <param name="calc">Word is mathematic expression</param>
        /// <param name="numeric">Word have a digits</param>
        public RecognizeParams(bool phrase = false, bool regsense = false, bool calc = false, bool numeric = false)
        {
            Params = new List<KeyValuePair<string, string>>();

            if (phrase)
            {
                Params.Add(new KeyValuePair<string, string>("phrase", "1"));
            }
            if (regsense)
            {
                Params.Add(new KeyValuePair<string, string>("regsense", "1"));
            }
            if (calc)
            {
                Params.Add(new KeyValuePair<string, string>("calc", "1"));
            }
            if (numeric)
            {
                Params.Add(new KeyValuePair<string, string>("numeric", "1"));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Antigate
    {
        #region Events

        public delegate void ErrorDel(ErrorState state);
        public delegate void RecognizedDel(string text);

        public static event ErrorDel OnError;
        public static event RecognizedDel OnRecognized;

        #endregion

        #region Variables

        const int RecognizeTime = 10000;
        const int RetryTime = 5000;
        const string ACCOUNT_ID = "da248030640620a5489c9b8d9475f507";
        public static Encoding StreamEncoding = Encoding.GetEncoding(1251);
        static Uri _postUri = new Uri("http://antigate.com/in.php");
        static string _resolveStr = "http://antigate.com/res.phpkey=da248030640620a5489c9b8d9475f507&action=get&id=";
        static string _boundary = "!@#$%^&*";

        #endregion

        /// <summary>
        /// Post image data on server
        /// </summary>
        /// <param name="image">Image data</param>
        /// <param name="type">Image extension type</param>
        /// <param name="param">Recognize params</param>
        public void PostImage(byte[] image, ImgType type, RecognizeParams param = null)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("Bad image data");

            List<KeyValuePair<string, string>> postParams = new List<KeyValuePair<string, string>>();
            postParams.Add(new KeyValuePair<string, string>("method", "post"));
            postParams.Add(new KeyValuePair<string, string>("key", ACCOUNT_ID));

            if (param!=null)
            {
                foreach (var keyValue in param.Params)
                {
                    postParams.Add(keyValue);
                }
            }

            byte[] fbData = BuldFormbasedData(postParams, StreamEncoding.GetString(image), type);

            string contentType = "multipart/form-data; boundary=" + _boundary;
            RequestParams reqParams = new RequestParams(null, null, contentType, null, true, "POST", StreamEncoding, false);
            DownloaderObj obj = new DownloaderObj(_postUri, EndPostImage, true, null, CookieOptions.Empty, 4, null, null, false, 1000, null, reqParams);
            obj.PostData = fbData;
            Downloader.Queue(obj);
        }

        /// <summary>
        /// Post data callback
        /// </summary>
        private void EndPostImage(DownloaderObj obj)
        {
            if (obj.DataStr != null)
            {
                HandlePostState(obj.DataStr);
            }
            else
            {
#if DEBUG
                GlobalLog.Err("null data in EndPostImage, POST_DATA:\n{0}", obj.PostData);
#endif
                Err(ErrorState.ConnectProblem);
            }
        }

        private void EndPostImage(IAsyncResult ar)
        {
            HttpWebRequest request = (ar.AsyncState as HttpWebRequest);
            try
            {
                HttpWebResponse response = request.EndGetResponse(ar) as HttpWebResponse;
                byte[] buffer = new byte[8 * 1024];
                byte[] data;
                using (Stream stream = response.GetResponseStream())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        int readed;
                        while ((readed = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, readed);
                        }
                        data = ms.GetBuffer();
                    }
                }
                string dataStr = StreamEncoding.GetString(data);
                HandlePostState(dataStr);
            }
            catch (Exception e)
            {
#if DEBUG
                GlobalLog.Err(e, "error while data reading");       
#endif
                Err(ErrorState.ConnectProblem);
            }
        }

        private void AskRecognize(string id)
        {
            Uri resolveUri = new Uri(_resolveStr + id);
            DownloaderObj obj = new DownloaderObj(resolveUri, null, true, null, CookieOptions.Empty, 5);
            Downloader.DownloadSync(obj);
            if (obj.DataStr != null)
            {
                HandleRecognizeState(obj.DataStr, id);
            }
            else
            {
#if DEBUG
                GlobalLog.Err("can't ASK data, id:{0}", id);       
#endif
                Err(ErrorState.ConnectProblem);
            }
        }

        private void AskRecognize(object arg)
        {
            object[] args = arg as object[];
            string id = args[1] as string;
            Timer timer = args[0] as Timer;

            Uri resolveUri = new Uri(_resolveStr + id);
            DownloaderObj obj = new DownloaderObj(resolveUri, null, true, null, CookieOptions.Empty, 5);
            Downloader.DownloadSync(obj);
            if (obj.DataStr != null)
            {
                HandleRecognizeState(obj.DataStr, id);
            }
            else
            {
                Err(ErrorState.ConnectProblem);
            }
            timer.Dispose();
        }

        #region Form-based construction

        /// <summary>
        /// Build formbased post data source
        /// </summary>
        /// <param name="keyValueParams">Recognize params</param>
        /// <param name="image">Image data as string</param>
        /// <param name="imgType">Image extension type</param>
        /// <returns>Formbase data buffer</returns>
        private byte[] BuldFormbasedData(List<KeyValuePair<string, string>> keyValueParams, string image, ImgType imgType)
        {
            StringBuilder formatBasedStr = new StringBuilder();

            foreach (var pair in keyValueParams)
            {
                formatBasedStr.Append(BuldParamFormStr(_boundary, pair.Key, pair.Value));
            }
            formatBasedStr.Append(BuldFileFormStr(_boundary, imgType, image));
            formatBasedStr.AppendFormat("{0}{1}{0}", "--", _boundary);

            string final = formatBasedStr.ToString();

            return StreamEncoding.GetBytes(formatBasedStr.ToString());
        }

        private string BuldParamFormStr(string boundary, string name, string value)
        {
            string fs = string.Empty;
            fs += "--" + boundary + "\r\n";
            fs += "Content-Disposition: form-data; name=\"" + name + "\"\r\n\r\n";
            fs += value + "\r\n";
            return fs;
        }

        private string BuldFileFormStr(string boundary, ImgType imgType, string value)
        {
            string fs = string.Empty;
            fs += "--" + boundary + "\r\n";
            fs += "Content-Disposition: form-data; name=\"file\"; filename=\"image.jpeg\"\r\n";
            fs += "Content-Type: " + imgType + "\r\n\r\n";
            fs += value + "\r\n";
            return fs;
        }

        #endregion

        #region Responce data handling

        /// <summary>
        /// Handle server answer on image posting
        /// </summary>
        /// <param name="data">Server responce data</param>
        private void HandlePostState(string data)
        {
            if (data.Contains(State.OK))
            {
                string[] param = data.Split('|');
                if (param.Length < 2)
                {
                    Err(ErrorState.OkWithNoID);
                }
                else
                {
                    new Thread(() => { Thread.Sleep(RecognizeTime); AskRecognize(param[1].Replace("\0", "")); }).Start();
                }
            }
            else if (data.Contains(State.ERROR_WRONG_USER_KEY) || data.Contains(State.ERROR_KEY_DOES_NOT_EXIST))
            {
                Err(ErrorState.BadUserKey);
            }
            else if (data.Contains(State.ERROR_ZERO_BALANCE))
            {
                Err(ErrorState.NoMoney);
            }
            else if (data.Contains(State.ERROR_NO_SLOT_AVAILABLE))
            {
                Err(ErrorState.NoSlots);
            }
            else if (data.Contains(State.ERROR_ZERO_CAPTCHA_FILESIZE) || data.Contains(State.ERROR_TOO_BIG_CAPTCHA_FILESIZE))
            {
                Err(ErrorState.BadImageSize);
            }
            else if (data.Contains(State.ERROR_IP_NOT_ALLOWED))
            {
                Err(ErrorState.IpBanned);
            }
            else 
            {
                Err(ErrorState.BadImageFormat);
            }
        }

        /// <summary>
        /// Handle server answer on ask results
        /// </summary>
        /// <param name="data"></param>
        /// <param name="id"></param>
        private void HandleRecognizeState(string data, string id)
        {
            if (data.Contains(State.OK))
            {
                string[] param = data.Split('|');
                if (param.Length < 2)
                {
                    Err(ErrorState.OkWithNoText);
                }
                else
                {
                    Recognized(param[1].Replace("\0", ""));
                }
            }
            else if (data.Contains(State.CAPCHA_NOT_READY))
            {
                new Thread(() => { Thread.Sleep(RetryTime); AskRecognize(id); }).Start();
            }
            else if (data.Contains(State.ERROR_KEY_DOES_NOT_EXIST))
            {
                Err(ErrorState.BadUserKey);
            }
            else if (data.Contains(State.ERROR_WRONG_ID_FORMAT))
            {
                Err(ErrorState.BadCapchaID);
            }
        }

        #endregion

        private void Err(ErrorState state)
        {
            if (OnError != null)
            {
                OnError(state);
            }
        }

        private void Recognized(string code)
        {
            if (OnRecognized != null)
            {
                OnRecognized(code);
            }
        }
    }
}