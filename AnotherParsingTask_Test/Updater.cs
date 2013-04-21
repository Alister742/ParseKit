using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Data.Linq;
using ParseKit.CORE;
using System.Threading;
using ParseKit.Data.db;
using ParseKit.Data.DBWorkers;
namespace AnotherParsingTask_Test
{
    public class Updater
    {
        Regex _rx = new Regex(@"ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_ucImageView1_ItemDataView_ctl\d{2}_lnkTitleImage");

        object _databaseSync = new object();
        object _fileSync = new object();
        //List<string> _names;
        //List<string> _resuls = new List<string>();
        XLSWorker _xls;
        HashSet<string> _upc = new HashSet<string>();

        int _processedUrls;
        int _queuedUrls;
        int _processedCDs;
        int _queuedCDs;
        int _processedPictures;
        int _queuedPictures;

        string _imagePath;

        string url = "http://www.springtowndvd.com/Contents/Default.aspx?oristudioid=";

        public void Update(string imageFolderPath, string xlsPath)
        {
            Console.WriteLine("UPDATING");
            _imagePath = imageFolderPath;
            _xls = new XLSWorker(xlsPath);

            _xls.SelectColumn(2).ForEach(x => _upc.Add(x));
            Console.WriteLine("UPC Cache is up");
            n = 0;
            _queuedUrls = studios.Length;
            Downloader.MaxParallelRequests = 10;

            for (int i = 0; i < studios.Length; i++) 
            {
                DownloadCdsPage(url + studios[i], ScrabUrls);
            }

            Console.Read();
        }
        static int n;
        void DownloadCdsPage(string url, Action<DownloaderObj> callback)
        {
            Interlocked.Increment(ref n);
            Uri uri;
            if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
            {
                DownloaderObj obj = new DownloaderObj(uri, callback, true, null, CookieOptions.Take, 100);
                Downloader.Queue(obj);
                Console.WriteLine("Queued {0} url", n);
            }
        }

        void ScrabUrls(DownloaderObj obj)
        {
            if (obj.DataStr == null)
	            return;

            StreamWriter sw = new StreamWriter(@"c:\" + _processedUrls + "1.txt", true, Encoding.UTF8);
            sw.WriteLine(obj.DataStr);
            sw.Close();


            Interlocked.Increment(ref _processedUrls);
            Console.WriteLine("PROCESSED {0} urls of {1}", _processedUrls, _queuedUrls);

            if (string.IsNullOrEmpty(obj.DataStr))
                return;

            List<string> CDurls = new List<string>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(obj.DataStr);

            var aTags = doc.DocumentNode.SelectNodes("//a").Where(x => (x.Attributes["id"] != null && _rx.IsMatch(x.Attributes["id"].Value)));

            foreach (var node in aTags)
            {
                CDurls.Add("http://www.springtowndvd.com" + node.Attributes["href"].Value);
            }

            CDurls.ForEach((u) => { DownloadCDData(u, ScrabCDData); });
        }

        void DownloadCDData(string url, Action<DownloaderObj> callback)
        {
            Uri uri;
            if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
            {
                Interlocked.Increment(ref _queuedCDs);

                DownloaderObj obj = new DownloaderObj(uri, callback, true, null, CookieOptions.Take, 100);
                Downloader.Queue(obj);
            }
        }

        void ScrabCDData(DownloaderObj obj)
        {
            Interlocked.Increment(ref _processedCDs);
            Console.WriteLine("parsed {0} CDs of {1} queued", _processedCDs, _queuedCDs);

            /* FORMAT IS 
             * UPC | Availability | Release Date | Starring | Director | Studio | Language | Length | Series | Category | Price | Features | Description | Name
             */

            if (string.IsNullOrEmpty(obj.DataStr))
                return;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(obj.DataStr);

            string UPC = GetInnerText(doc, "ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_FormView1_lblItemCode");
            string Availability = GetInnerText(doc, "ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_FormView1_Label5");
            string ReleaseDate = GetInnerText(doc, "ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_FormView1_Label6");
            string Starring = GetInnerText(doc, "ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_FormView1_lstStarring_itemPlaceholderContainer");
            string Director = GetInnerText(doc, "ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_FormView1_lstDirector_itemPlaceholderContainer");
            string Studio = GetInnerText(doc, "ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_FormView1_Label12");
            string Language = GetInnerText(doc, "ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_FormView1_Label14");
            string Length = GetInnerText(doc, "ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_FormView1_Label16");
            string Series = GetInnerText(doc, "ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_FormView1_Label27");
            string Category = GetInnerText(doc, "ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_FormView1_lstCategoryView_itemPlaceholderContainer");

            string Features = string.Empty;
            string Description = string.Empty;
            HtmlNodeCollection infoArea = doc.DocumentNode.SelectNodes("//div[@id='additional_info_area']/div[@class='title_info']/ul/li/p");
            if (infoArea != null)
            {
                 Features = infoArea.Count > 0 ? infoArea[0].InnerText.DeleteEmpties() : string.Empty;
                 Description = infoArea.Count > 1 ? infoArea[1].InnerText.DeleteEmpties() : string.Empty;
            }

            string Name = string.Empty;

            HtmlNodeCollection nameArea = doc.DocumentNode.SelectNodes("//table[@id='ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_FormView1']/tr/td/h1");
            if (nameArea != null && nameArea.Count > 0)
            {
                Name = nameArea[0].InnerText.DeleteEmpties();
            }

            if (!_upc.Contains(UPC))
            {
                lock (_databaseSync)
                {
                    try
                    {
                        _xls.InsertLastLine(new List<string>(new string[] { 
                    "-1",
                    UPC, 
                    Availability, 
                    ReleaseDate, 
                    Starring, 
                    Director, 
                    Studio, 
                    Language, 
                    Length,
                    Series, 
                    Category, 
                    Features, 
                    Description, 
                    Name}),
0);
                    }
                    catch (Exception)
                    {
                        return;
                        
                    }

                }

                if (!string.IsNullOrEmpty(UPC) && !File.Exists(_imagePath + UPC + "f.jpg"))
                {
                    Interlocked.Increment(ref _queuedPictures);
                    Downloader.Queue(new DownloaderObj(new Uri("http://www.springtowndvd.com/images/secure/zoom/" + UPC + "f.jpg"), SaveImage, false, null, CookieOptions.Take, 100, UPC));
                }
            }

            if (_processedCDs == _queuedCDs)
            {
                _xls.Dispose();
                Console.WriteLine();
            }
        }

        public void SaveImage(DownloaderObj obj)
        {
            Interlocked.Increment(ref _processedPictures);
            Console.WriteLine("downloaded {0} pictures of {1} queued, path {2}", _processedPictures, _queuedPictures, _imagePath);

            if (obj.Data == null)
                return;

            using (FileStream fs = File.Create(_imagePath + obj.Arg as string + "f.jpg"))
            {
                fs.Write(obj.Data, 0, obj.Data.Length);
            }

            if (_processedPictures == _queuedPictures)
            {
                Console.WriteLine("Done");   
            }
        }

        #region Support stuff

        //bool IsNew(string s)
        //{
        //    return false;
        //}

        string GetInnerText(HtmlDocument doc, string id)
        {
            HtmlNode node = doc.GetElementbyId(id);
            return node != null ? node.InnerText.DeleteEmpties() : string.Empty;
        }

        //byte[] GetData(Uri uri)
        //{
        //    HttpWebRequest req = HttpWebRequest.Create(uri) as HttpWebRequest;

        //    req.UserAgent = "Opera/9.80 (Windows NT 5.1) Presto/2.12.388 Version/12.12";
        //    req.Host = uri.Host;
        //    req.Accept = "text/html, application/xml;q=0.9, application/xhtml+xml, image/png, image/webp, image/jpeg, image/gif, image/x-xbitmap, */*;q=0.1";
        //    req.Headers.Add("Accept-Language", "en;q=0.8");
        //    req.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.Deflate;

        //    HttpWebResponse resp = req.GetResponse() as HttpWebResponse;

        //    byte[] data;
        //    using (Stream respStream = resp.GetResponseStream())
        //    {
        //        using (MemoryStream dataStream = new MemoryStream())
        //        {
        //            byte[] buff = new byte[4 * 1024];
        //            int i = -1;

        //            while ((i = respStream.Read(buff, 0, buff.Length)) > -1)
        //            {
        //                dataStream.Write(buff, 0, i);
        //            }
        //            data = dataStream.ToArray();
        //        }
        //    }

        //    return data;
        //}

        //string GetString(Uri uri)
        //{
        //    byte[] buff = GetData(uri);

        //    string data = Encoding.UTF8.GetString(buff);

        //    return data;
        //}

        //void SaveTofile(string filePath, bool append, List<string> lines)
        //{
        //    lock (_fileSync)
        //    {
        //        using (StreamWriter sw = new StreamWriter(filePath, append, Encoding.UTF8))
        //        {
        //            for (int i = 0; i < lines.Count; i++)
        //            {
        //                sw.WriteLine(lines[i]);
        //            }
        //        }
        //    }
        //}
        #endregion

        #region Studios
        string[] studios = new string[] {"THIR",
                                                "ABBY",
                                                "ABSG",
                                                "ADAM",
                                                "ADDG",
                                                "ADUL",
                                                "AMAT",
                                                "AMAX",
                                                "AMKE",
                                                "ASSN",
                                                "BANG",
                                                "BLAG",
                                                "BLAI",
                                                "BLAP",
                                                "BLAK",
                                                "BLAB",
                                                "BLUC",
                                                "BLUB",
                                                "BOND",
                                                "BRAZ",
                                                "BRAG",
                                                "BURN",
                                                "CEZA",
                                                "CHAN",
                                                "CJEN",
                                                "COMB",
                                                "DARD",
                                                "DARK",
                                                "DEVI",
                                                "DIAB",
                                                "DPLY",
                                                "DSIN",
                                                "DOGF",
                                                "DOUT",
                                                "DREZ",
                                                "DGIR",
                                                "DVSX",
                                                "ELEG",
                                                "EVAS",
                                                "EVIL",
                                                "EVOL",
                                                "EXQU",
                                                "FALL",
                                                "FETC",
                                                "FILL",
                                                "FREP",
                                                "GIRC",
                                                "GIRL",
                                                "HARA",
                                                "HOMA",
                                                "HUSH",
                                                "HUST",
                                                "IMMO",
                                                "INNO",
                                                "JMPR",
                                                "JOYB",
                                                "JUIC",
                                                "JULE",
                                                "JSIX",
                                                "KICK",
                                                "legn",
                                                "LETH",
                                                "LOVK",
                                                "MADH",
                                                "MAIK",
                                                "MANC",
                                                "MERC",
                                                "MOFO",
                                                "MORA",
                                                "MUFF",
                                                "MYSE",
                                                "NAUG",
                                                "NEWS",
                                                "NEWP",
                                                "NORT",
                                                "OVER",
                                                "PLAM",
                                                "PLAT",
                                                "PLEA",
                                                "PORP",
                                                "PORC",
                                                "PULS",
                                                "PUSC",
                                                "REAK",
                                                "REDL",
                                                "REDI",
                                                "RODN",
                                                "SCOR",
                                                "SENS",
                                                "SEYM",
                                                "SHOO",
                                                "SHOT",
                                                "SILS",
                                                "SIMW",
                                                "SINL",
                                                "SMAS",
                                                "STIC",
                                                "SWAN",
                                                "TTBS",
                                                "TEAS",
                                                "THIW",
                                                "tomb",
                                                "TRAN",
                                                "TWIY",
                                                "ULTI",
                                                "VENU",
                                                "VIDE",
                                                "VINV",
                                                "VINC",
                                                "VIVT",
                                                "vivd",
                                                "viva",
                                                "VOOD",
                                                "VOUY",
                                                "WCPX",
                                                "WHIG",
                                                "WICK",
                                                "WILD",
                                                "ZERO",};
        #endregion
    }
}
