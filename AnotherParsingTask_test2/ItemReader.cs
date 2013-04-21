using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using ParseKit.Data;
using System.Text.RegularExpressions;
using System.IO;
using ParseKit.CORE;
using System.Threading;
using ParseKit.Interfaces;

namespace AnotherParsingTask_test2
{
    public class ItemReader : IPageReader
    {
        #region Члены IPageReader

        public event NewTargetsDel OnNewTargets;
        public event ReadCompleteDel OnReadComplete;

        string Normalize(string data)
        {
            return data.Trim();
        }

        public void ReadData(string data, DevourTarget target)
        {
            try
            {

                data = HttpUtility.HtmlDecode(data);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(data);

                string Title = string.Empty;

                string MediumTyp = string.Empty;
                string ArtNr = string.Empty;
                string Shopdatum = string.Empty;
                string Seitenbesucher = string.Empty;
                string Studio = string.Empty;
                string Laufzeit = string.Empty;
                string Bildqualitat = string.Empty;
                string Erscheinungsdatum = string.Empty;
                string Sprachen = string.Empty;

                string Price = string.Empty;

                string ImagePath_One = string.Empty;
                string ImagePath_Two = string.Empty;

                string Description = string.Empty;
                string Genre = string.Empty;
                string Authors = string.Empty;

                HtmlNodeCollection titleArea = doc.DocumentNode.SelectNodes("//div[@id='page_contentmaster']/h1");
                if (titleArea != null && titleArea.Count > 0)
                {
                    Title = Normalize(titleArea[0].InnerText.Replace("Titel:", ""));
                }

                HtmlNodeCollection itemcontentArea = doc.DocumentNode.SelectNodes("//div[@id='page_contentmaster']/div[@class='Itemcontent']/div[@style]");

                if (itemcontentArea != null && itemcontentArea.Count > 0)
                {
                    HtmlNode details = itemcontentArea[0];

                    string detailsHtml = details.InnerHtml;

                    string[] detailsItems = detailsHtml.Split(new string[] { "<br>" }, StringSplitOptions.None);

                    for (int i = 0; i < detailsItems.Length; i++)
                    {
                        string[] s = Normalize(detailsItems[i]).Split('>');
                        for (int j = 0; j < s.Length - 1; j++)
                        {
                            if (s[j].Contains("Medium Typ"))
                            {
                                MediumTyp = Normalize(s[s.Length - 1]);
                            }

                            if (s[j].Contains("Art.Nr."))
                            {
                                ArtNr = Normalize(s[s.Length - 1]);
                                if (Updater.ArtNr.Contains(ArtNr))
                                {
                                    if (OnReadComplete != null)
                                    {
                                        OnReadComplete(target);
                                    }
                                    return;
                                }
                            }

                            if (s[j].Contains("Shopdatum"))
                            {
                                Shopdatum = Normalize(s[s.Length - 1]);
                            }

                            if (s[j].Contains("Seitenbesucher"))
                            {
                                Seitenbesucher = Normalize(s[s.Length - 1]);
                            }

                            if (s[j].Contains("Studio"))
                            {
                                MatchCollection ss = new Regex(">(?<studio>[^<]*)<", RegexOptions.IgnoreCase).Matches(detailsItems[i]);
                                if (ss.Count > 0)
                                {
                                    Studio = Normalize(ss[ss.Count - 1].Groups["studio"].Value);
                                }
                            }

                            if (s[j].Contains("Laufzeit"))
                            {
                                Laufzeit = Normalize(s[s.Length - 1]);
                            }

                            if (s[j].Contains("Bildqualit"))
                            {
                                Bildqualitat = new Regex("img", RegexOptions.IgnoreCase).Matches(detailsItems[i]).Count.ToString();
                            }

                            if (s[j].Contains("Erscheinungsdatum"))
                            {
                                Erscheinungsdatum = Normalize(s[s.Length - 1]);
                            }

                            if (s[j].Contains("Sprachen"))
                            {
                                Sprachen = Normalize(s[s.Length - 1]);
                            }
                        }
                    }
                    #region Description, Genre, Dartseller
                    if (itemcontentArea.Count > 1)
                    {
                        foreach (var descriptionNode in itemcontentArea)
                        {
                            if (descriptionNode.GetAttributeValue("style", "") == "padding-left: 30px; text-align: left; width: 90%;")
                            {
                                HtmlNodeCollection bTagCells = descriptionNode.SelectNodes("./b");

                                bool haveGenre = false;
                                foreach (var bTagCell in bTagCells)
                                {
                                    if (bTagCell.InnerText.Contains("Genre"))
                                    {
                                        haveGenre = true;
                                        break;
                                    }
                                }

                                bool haveDarsteller = false;
                                foreach (var bTagCell in bTagCells)
                                {
                                    if (bTagCell.InnerText.Contains("Darsteller"))
                                    {
                                        haveDarsteller = true;
                                        break;
                                    }
                                }

                                if (haveGenre)
                                {
                                    HtmlNode genreSpan = descriptionNode.SelectNodes("./span[@class='c10']")[0];
                                    if (genreSpan != null)
                                    {
                                        HtmlNodeCollection genreLinks = genreSpan.SelectNodes("./a");

                                        if (genreLinks != null)
                                        {
                                            foreach (var genre in genreLinks)
                                            {
                                                if (!Genre.Contains(genre.InnerText.Trim()))
                                                {
                                                    Genre += Genre != string.Empty ? ", " + genre.InnerText.Trim() : genre.InnerText.Trim();
                                                }
                                            }
                                        }
                                    }
                                }

                                if (haveDarsteller)
                                {
                                    HtmlNodeCollection allLinks = descriptionNode.SelectNodes("./span[@class='c10']/a");

                                    if (allLinks != null)
                                    {
                                        int firstDartsellerLinkIndex = haveGenre ? 1 : 0;
                                        for (int i = firstDartsellerLinkIndex; i < allLinks.Count; i++)
                                        {
                                            Authors += Authors != string.Empty ? ", " + allLinks[i].InnerText.Trim() : allLinks[i].InnerText.Trim();
                                        }
                                    }
                                }
                            }
                        }


                        HtmlNode mainDescriptionNode = itemcontentArea[itemcontentArea.Count - 1];

                        string uri = target.Uri.OriginalString;

                        if (mainDescriptionNode != null)
                        {
                            foreach (var item in mainDescriptionNode.ChildNodes)
                            {
                                if (item.Name != "a" && item.Name != "b" && item.Name != "span")
                                {
                                    string descriptionLine = Normalize(item.InnerText);
                                    if (!string.IsNullOrEmpty(descriptionLine))
                                    {
                                        Description += descriptionLine + "\r\n";
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                }

                HtmlNodeCollection priceArea = doc.DocumentNode.SelectNodes("//div[@class='Itemcontent']/div[@style]/p[@style]/span[@class='brownb']");
                if (priceArea != null && priceArea.Count > 0)
                {
                    Price = Normalize(priceArea[0].InnerText).Replace("&#128;", "€");
                }

                HtmlNodeCollection imagesArea = doc.DocumentNode.SelectNodes("//div[@class='image_div3']/a[@class='screenshot2']/img[@class='photoframe']");
                if (imagesArea != null && imagesArea.Count > 0)
                {
                    ImagePath_One = imagesArea[0].GetAttributeValue("src", "");
                }
                if (imagesArea != null && imagesArea.Count > 1)
                {
                    ImagePath_Two = imagesArea[1].GetAttributeValue("src", "");
                }

                Uri image_one_uri;
                if ((image_one_uri = UriHandler.CreateUri(ImagePath_One)) != null)
                {
                    DownloaderObj image_one_obj = new DownloaderObj(image_one_uri, ImagDownloadCallback, false, null, CookieOptions.Take, 100, Path.GetFileName(ImagePath_One));
                    Downloader.Queue(image_one_obj);
                }

                Uri image_two_uri;
                if ((image_two_uri = UriHandler.CreateUri(ImagePath_Two)) != null)
                {
                    DownloaderObj image_two_obj = new DownloaderObj(image_two_uri, ImagDownloadCallback, false, null, CookieOptions.Take, 100, Path.GetFileName(ImagePath_One));
                    Downloader.Queue(image_two_obj);
                }


                Updater.SaveData(
                    Title,
                    MediumTyp,
                    ArtNr,
                    Shopdatum,
                    Seitenbesucher,
                    Studio,
                    Laufzeit,
                    Bildqualitat,
                    Erscheinungsdatum,
                    Sprachen,
                    Genre,
                    Price,
                    Description,
                    Path.GetFileName(ImagePath_One),
                    Path.GetFileName(ImagePath_Two),
                    Authors);

                //save all data!!!!

                if (OnReadComplete != null)
                {
                    OnReadComplete(target);
                }

                Console.WriteLine("{0} Total page scrapped", Interlocked.Increment(ref _globalPagesScrapped));
            }
            catch (Exception e)
            {
                Console.WriteLine("> Exception: {0}", e.Message);
            }
        }

        static int _globalPagesScrapped;

        void ImagDownloadCallback(DownloaderObj obj)
        {
            Console.WriteLine("image downloaded, total queued for download {0}", Downloader.Queued);
            Updater.SaveImage(obj.Data, obj.Arg as string);
        }

        #endregion
    }
}
