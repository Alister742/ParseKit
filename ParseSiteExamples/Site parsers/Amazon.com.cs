using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ParseKit.Data;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using ParseKit.CORE;

namespace ParseKit.Parsers
{
    class Amazon
    {
        public Uri Target = new Uri("http://www.amazon.com/gp/bestsellers/");
        //Devourer pageDownl = new Devourer();

        Regex _menuRx = new Regex("<ul id=\"zg_browseRoot\">(?<menu>[\\s|\\S]*?)(?=</ul>)", RegexOptions.Compiled);
        Regex _productRx = new Regex("<div class=\"zg_title\"><a[^\"]*\"\\s*(?<product>[^\"]*)\"", RegexOptions.Compiled);
        Regex _ASINRx = new Regex("<li><b>ASIN:</b>\\s*(?<ASIN>[\\S]*?)\\s*<", RegexOptions.Compiled);
        Regex _linkRefRx = new Regex("ref=[\\s|\\S]*", RegexOptions.Compiled);

        public void HarvestAllBestsellersASIN()
        {
            List<Uri> allCategories = GetAllCategories(Target);

            StreamWriter sw = new StreamWriter(@"c:\categories", false, Encoding.UTF8);
            foreach (var categoryUri in allCategories)
            {
                sw.WriteLine(categoryUri.OriginalString);
            }
            sw.Close();

            GC.Collect();
        }

        void SaveCategory(string categoryUri)
        {
            StreamWriter sw = new StreamWriter(@"c:\categories", true, Encoding.UTF8);
            sw.WriteLine(categoryUri);
            sw.Close();
        }

        void SaveProducts(List<Uri> productsUris)
        {
            StreamWriter sw = new StreamWriter(@"c:\products", true, Encoding.UTF8);
            foreach (var productUri in productsUris)
            {
                sw.WriteLine(productUri.OriginalString);
            }
            sw.Close();
        }

        void SaveASIN(string ASIN)
        {
            StreamWriter sw = new StreamWriter(@"c:\ASIN", true, Encoding.UTF8);
            sw.WriteLine(ASIN);
            sw.Close();
        }

        #region AllCategoriesParse

        object allCategoriesSync = new object();
        object tempListSync = new object();

        HashSet<string> allCategories = new HashSet<string>();
        List<Uri> tempList = new List<Uri>();

        WaitObj waiter = new WaitObj(0);

        public List<Uri> GetAllCategories(Uri pageWithMenu)
        {            
            allCategories.Add(pageWithMenu.OriginalString);

            int curIndx = 16900;
            //LAST SAVE
            StreamReader sr = new StreamReader(@"c:\categories", Encoding.UTF8);
            while (!sr.EndOfStream)
            {
                allCategories.Add(sr.ReadLine());
            }
            sr.Close();
            tempList.Add(pageWithMenu);
            tempList.AddRange(allCategories.Select<string, Uri>((string uri) => { return new Uri(uri); }).ToList());
            //////////////////////////

            while (curIndx < tempList.Count)
            {
                #region AsyncAlg
                int maxOr50 = tempList.Count - curIndx > 50 ? 50 : tempList.Count - curIndx;
                waiter.Count = maxOr50;
                for (int i = 0; i < maxOr50; i++)
                {
                    AsyncDownloadCategoryPage(curIndx + i);
                }
                curIndx += maxOr50;
                waiter.WaitEvent.WaitOne();
                #endregion

                #region SyncAlg
                //DownloaderObj obj = new DownloaderObj(tempList[curIndx], null, true, null, false);
                //Downloader.DownloadSync(obj);
                //if (obj.DataStr != null)
                //{
                //    string pageMenuData = GetMenuDataPiece(obj.DataStr, tempList[curIndx]);
                //    curCategories = GetAllMenuLinks(pageMenuData, tempList[curIndx]);
                //}
                //foreach (var category in curCategories)
                //{
                //    string clearCategory = _linkRefRx.Replace(category, "");
                //    if (!allCategories.Contains(clearCategory))
                //    {
                //        allCategories.Add(clearCategory);
                //        SaveCategory(clearCategory);
                //        Console.WriteLine("Total collected categoryes - {0}", allCategories.Count);
                //        tempList.Add(new Uri(clearCategory));
                //    }
                //}
                //tempList[curIndx] = null;
                //curIndx++;
                #endregion
                Console.WriteLine("Indx {0} of {1}", curIndx, tempList.Count);
            }
            return allCategories.Select<string, Uri>((string uri) => { return new Uri(uri); }).ToList();
        }

        void AsyncDownloadCategoryPage(int curIndx)
        {
            ThreadPool.QueueUserWorkItem((object o) =>
                {
                    HashSet<string> curCategories = new HashSet<string>();
                    DownloaderObj obj = new DownloaderObj(tempList[curIndx], null, true, null, CookieOptions.NoCookies, 1000);
                    Downloader.DownloadSync(obj);
                    if (obj.DataStr != null)
                    {
                        string pageMenuData = GetMenuDataPiece(obj.DataStr, tempList[curIndx]);
                        curCategories = GetAllMenuLinks(pageMenuData, tempList[curIndx]);
                    }
                    foreach (var category in curCategories)
                    {
                        string clearCategory = _linkRefRx.Replace(category, "");
                        lock (allCategoriesSync)
                        {
                            lock (tempListSync)
                            {
                                if (!allCategories.Contains(clearCategory))
                                {
                                    allCategories.Add(clearCategory);
                                    SaveCategory(clearCategory);
                                    Console.WriteLine("Total collected categoryes - {0}", allCategories.Count);
                                    tempList.Add(new Uri(clearCategory));
                                }
                            }
                        }
                    }
                    tempList[curIndx] = null;
                    Console.WriteLine("waiter.Count - " + waiter.Count);
                    if (Interlocked.Decrement(ref waiter.Count) == 0)
                    {
                        waiter.WaitEvent.Set();
                    }
                });
        }

        string GetMenuDataPiece(string page, Uri pageUri)
        {
            return _menuRx.Match(page).Groups["menu"].Value;
        }

        HashSet<string> GetAllMenuLinks(string menuData, Uri pageUri)
        {
            //BREAKED
            return null; //pageDownl.ParsePageUrls(menuData, pageUri);
        }

        #endregion

        #region AllProductsParse

        List<Uri> allProductsUrls = new List<Uri>();
        object allProductsUrlsSync = new object();

        public List<Uri> GetAllCategoriesProducts(List<string> categories)
        {
            
            int indx = 0;
            Downloader.MaxParallelRequests = 20;
            foreach (var category in categories)
            {
                for (int i = 1; i < 6; i++)
                {
                    DownloaderObj obj = new DownloaderObj(new Uri(category + "?pg=" + i), GetProductCallback, true, null, CookieOptions.NoCookies, 100, indx);
                    Downloader.Queue(obj);
                }
                indx++;
            }
            return allProductsUrls;
        }

        void GetProductCallback(DownloaderObj obj)
        {
            if (obj.DataStr != null)
            {
                List<Uri> products = GetProductsUrls(obj.DataStr);
                lock (allProductsUrlsSync)
                {
                    allProductsUrls.AddRange(products);
                    SaveProducts(products);
                }
                Console.WriteLine("Found {0} products on categoryIndx {1}", allProductsUrls.Count, (int)obj.Arg);
            }
        }

        List<Uri> GetProductsUrls(string categoryPage)
        {
            List<Uri> productsUrls = new List<Uri>();
            MatchCollection categoriesMatchs =  _productRx.Matches(categoryPage);
            foreach (Match ctgryMatch in categoriesMatchs)
            {
                 string product = _linkRefRx.Replace(ctgryMatch.Groups["product"].Value, "");
                 productsUrls.Add(new Uri(product));
            }
            return productsUrls;
        }

        #endregion

        #region ASINSCollecting

        List<string> CollectAllASINs(List<Uri> productsUrls)
        {
            List<string> ASINs = new List<string>();
            foreach (var productUrl in productsUrls)
            {
                DownloaderObj obj = new DownloaderObj(productUrl, null, true, null, CookieOptions.NoCookies, 100);
                Downloader.DownloadSync(obj);
                if (obj.DataStr != null)
                {
                    string ASIN = GetProductASIN(obj.DataStr);
                    if (!string.IsNullOrEmpty(ASIN))
                    {
                        ASINs.Add(ASIN);
                    }
                }
            }
            return ASINs;
        }

        string GetProductASIN(string productPage)
        {
            return _ASINRx.Match(productPage).Groups["ASIN"].Value;
        }

        #endregion
    }
}
