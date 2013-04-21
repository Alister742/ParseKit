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
using System.Diagnostics;

namespace AnotherParsingTask_Test
{
    class Program
    {
        static Regex _rx = new Regex(@"ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_ucImageView1_ItemDataView_ctl\d{2}_lnkTitleImage");

        static object _databaseSync = new object();
        static object _fileSync = new object();
        static ADONETWorker _nw;

        static int _processedUrls;
        static int _queuedUrls;
        static int _processedCDs;
        static int _queuedCDs;
        static int _processedPictures;
        static int _queuedPictures;
        static string _filename;

        static void Main(string[] args)
        {
            try
            {
                Process proc = Process.GetProcessesByName("EXCEL")[0];
                proc.Kill();
            }
            catch (Exception)
            {
            }


            Console.WriteLine("Image path is: {0}, data path is: {1}", args[0], args[1]);
            new Updater().Update(args[0], args[1]);

            //_nw = new ADONETWorker("BARDABARD", true);

            //DataWorkerConverter converter = new DataWorkerConverter();
            //converter.FromSQLToXLS(_nw, @"c:\studios.xls").Dispose();

            //Console.WriteLine("DONE!");
            Console.Read();
        }

        //static void RedownloadPictures(List<string> upcs)
        //{
        //    foreach (var upc in upcs)
        //    {
        //        if (!File.Exists(@"c:\!Работа\STUDIOS\PICTURES\" + upc + "f.jpg"))
        //        {
        //            Interlocked.Increment(ref _queuedPictures);
        //            Downloader.Queue(new DownloaderObj(new Uri("http://www.springtowndvd.com/images/secure/zoom/" + upc + "f.jpg"), SaveImage, false, null, CookieOptions.Take, 100, upc));
        //        }
        //    }
        //}

        //static void Start(string[] lines, int start, int end)
        //{
        //    Console.WriteLine("STARTED");
        //    for (int i = start; i < end; i++) //foreach line int 700 lines do:
        //    {
        //        DownloadCdsPage(lines[i], ScrabUrls);
        //    }
        //}

        //static void DownloadCdsPage(string url, Action<DownloaderObj> callback)
        //{
        //    Uri uri;
        //    if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
        //    {
        //        DownloaderObj obj = new DownloaderObj(uri, callback, true, null, CookieOptions.Take, 100);
        //        Downloader.Queue(obj);
        //    }
        //}

        //static void ScrabUrls(DownloaderObj obj)
        //{
        //    Interlocked.Increment(ref _processedUrls);
        //    Console.WriteLine("PROCESSED {0} urls of {1}", _processedUrls, _queuedUrls);

        //    if (string.IsNullOrEmpty(obj.DataStr))
        //        return;

        //    List<string> CDurls = new List<string>();

        //    HtmlDocument doc = new HtmlDocument();
        //    doc.LoadHtml(obj.DataStr);

        //    var aTags = doc.DocumentNode.SelectNodes("//a").Where(x => (x.Attributes["id"] != null && _rx.IsMatch(x.Attributes["id"].Value)));

        //    foreach (var node in aTags)
        //    {
        //        CDurls.Add("http://www.springtowndvd.com" + node.Attributes["href"].Value);
        //    }

        //    CDurls.ForEach((u) => { DownloadCDData(u, ScrabCDData); });
        //}

        //static void DownloadCDData(string url, Action<DownloaderObj> callback)
        //{
        //    Uri uri;
        //    if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
        //    {
        //        Interlocked.Increment(ref _queuedCDs);

        //        DownloaderObj obj = new DownloaderObj(uri, callback, true, null, CookieOptions.Take, 100);
        //        Downloader.Queue(obj);
        //    }
        //}

        //static void ScrabCDData(DownloaderObj obj)
        //{
        //    Interlocked.Increment(ref _processedCDs);
        //    Console.WriteLine("parsed {0} CDs of {1} queued", _processedCDs, _queuedCDs);

        //    /* FORMAT IS 
        //     * UPC | Availability | Release Date | Starring | Director | Studio | Language | Length | Series | Category | Price | Features | Description
        //     */

        //    if (string.IsNullOrEmpty(obj.DataStr))
        //        return;

        //    HtmlDocument doc = new HtmlDocument();
        //    doc.LoadHtml(obj.DataStr);

        //    string UPC = GetInnerText(doc, "ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_FormView1_lblItemCode");
        //    string Availability = GetInnerText(doc, "ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_FormView1_Label5");
        //    string ReleaseDate = GetInnerText(doc, "ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_FormView1_Label6");
        //    string Starring = GetInnerText(doc, "ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_FormView1_lstStarring_itemPlaceholderContainer");
        //    string Director = GetInnerText(doc, "ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_FormView1_lstDirector_itemPlaceholderContainer");
        //    string Studio = GetInnerText(doc, "ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_FormView1_Label12");
        //    string Language = GetInnerText(doc, "ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_FormView1_Label14");
        //    string Length = GetInnerText(doc, "ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_FormView1_Label16");
        //    string Series = GetInnerText(doc, "ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_FormView1_Label27");
        //    string Category = GetInnerText(doc, "ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_FormView1_lstCategoryView_itemPlaceholderContainer");

        //    string Features = string.Empty;
        //    string Description = string.Empty;
        //    HtmlNodeCollection infoArea = doc.DocumentNode.SelectNodes("//div[@id='additional_info_area']/div[@class='title_info']/ul/li/p");
        //    if (infoArea != null)
        //    {
        //        Features = infoArea.Count > 0 ? infoArea[0].InnerText.DeleteEmpties() : string.Empty;
        //        Description = infoArea.Count > 1 ? infoArea[1].InnerText.DeleteEmpties() : string.Empty;
        //    }

        //    string Name = string.Empty;

        //    HtmlNodeCollection nameArea = doc.DocumentNode.SelectNodes("//table[@id='ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder1_FormView1']/tr/td/h1");
        //    if (nameArea != null && nameArea.Count > 0)
        //    {
        //        Name = nameArea[0].InnerText.DeleteEmpties();
        //    }

        //    lock (_databaseSync)
        //    {
        //        _nw.Insert(new string[] { 
        //            UPC, 
        //            Availability, 
        //            ReleaseDate, 
        //            Starring, 
        //            Director, 
        //            Studio, 
        //            Language, 
        //            Length,
        //            Series, 
        //            Category, 
        //            Features, 
        //            Description, 
        //            Name },
        //            "Main");
                    
        //            //_nw.Insert(new string[] { 
        //        //    m.UPC, 
        //        //    m.Availability, 
        //        //    m.ReleaseDate, 
        //        //    m.Starring, 
        //        //    m.Director, 
        //        //    m.Studio, 
        //        //    m.Language, 
        //        //    m.Length,
        //        //    m.Series, 
        //        //    m.Category, 
        //        //    m.Features, 
        //        //    m.Description, 
        //        //    m.Name 
        //        //}, "Main");
        //    }

        //    if (!string.IsNullOrEmpty(UPC) && !File.Exists(@"c:\!Работа\STUDIOS\PICTURES\" + UPC + "f.jpg"))
        //    {
        //        Interlocked.Increment(ref _queuedPictures);
        //        Downloader.Queue(new DownloaderObj(new Uri("http://www.springtowndvd.com/images/secure/zoom/" + UPC + "f.jpg"), SaveImage, false, null, CookieOptions.Take, 100, UPC));
        //    }
        //}

        //public static void SaveImage(DownloaderObj obj)
        //{
        //    Interlocked.Increment(ref _processedPictures);
        //    Console.WriteLine("downloaded {0} pictures of {1} queued", _processedPictures, _queuedPictures);

        //    if (obj.Data == null)
        //        return;

        //    using (FileStream fs = File.Create(@"c:\!Работа\STUDIOS\PICTURES\" + obj.Arg as string + "f.jpg"))
        //    {
        //        fs.Write(obj.Data, 0, obj.Data.Length);
        //    }
        //}

        #region Support stuff

        static bool IsNew(string s)
        {
            return false;
        }

        static string GetInnerText(HtmlDocument doc, string id)
        {
            HtmlNode node = doc.GetElementbyId(id);
            return node != null ? node.InnerText.DeleteEmpties() : string.Empty;
        }

        static byte[] GetData(Uri uri)
        {
            HttpWebRequest req = HttpWebRequest.Create(uri) as HttpWebRequest;

            req.UserAgent = "Opera/9.80 (Windows NT 5.1) Presto/2.12.388 Version/12.12";
            req.Host = uri.Host;
            req.Accept = "text/html, application/xml;q=0.9, application/xhtml+xml, image/png, image/webp, image/jpeg, image/gif, image/x-xbitmap, */*;q=0.1";
            req.Headers.Add("Accept-Language", "en;q=0.8");
            req.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.Deflate;

            HttpWebResponse resp = req.GetResponse() as HttpWebResponse;

            byte[] data;
            using (Stream respStream = resp.GetResponseStream())
            {
                using (MemoryStream dataStream = new MemoryStream())
                {
                    byte[] buff = new byte[4 * 1024];
                    int i = -1;

                    while ((i = respStream.Read(buff, 0, buff.Length)) > -1)
                    {
                        dataStream.Write(buff, 0, i);
                    }
                    data = dataStream.ToArray();
                }
            }

            return data;
        }

        static string GetString(Uri uri)
        {
            byte[] buff = GetData(uri);

            string data = Encoding.UTF8.GetString(buff);

            return data;
        }

        static void SaveTofile(string filePath, bool append, string line)
        {
            lock (_fileSync)
            {
                using (StreamWriter sw = new StreamWriter(filePath, append, Encoding.UTF8))
                {
                    sw.WriteLine(line);
                }
            }
        }
        #endregion
    }






    static class StringExt
    {
        static Regex emp = new Regex(@"\ {2,}|[\f\n\r\t\v]");

        public static string DeleteEmpties(this string s)
        {
            return emp.Replace(s, "").Replace("&nbsp;", " ").Replace("&#44;", ",");
        }
    }
}
