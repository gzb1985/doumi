using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using System.Xml;
using System.Windows;
using System.Net.NetworkInformation;
using BookInfo;
using System.Windows.Resources;

namespace WebHelpers
{
    public class WebHelper
    {
        static WebHelper instance = null;

        public WebHelper()
        {
        }

        public static WebHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WebHelper();
                }
                return instance;
            }
        }

        public void DownloadString(string url, Action<string> onDownloadCompleted, Action onConnectionFailed, Action<int> onProgressChanged)
        {
            try
            {
                var webClient = new WebClient();
                webClient.DownloadStringCompleted += (sender, e) =>
                    {
                        try
                        {
                            onDownloadCompleted(e.Result);
                        }
                        catch (Exception exc)
                        {
                            //ShowUnexpectedError();
                            onConnectionFailed();
                        }
                    };
                webClient.DownloadProgressChanged += (sender, e) =>
                    {
                        onProgressChanged(e.ProgressPercentage);
                    };
                webClient.AllowReadStreamBuffering = true;
                webClient.DownloadStringAsync(new Uri(url), webClient);
            }
            catch (Exception exc)
            {
                //ShowUnexpectedError();
                onConnectionFailed();
            }
        }


        public void DownloadBookInfo(string url, Action<Book> onContentReady, Action onConnectionFailed, Action<int> onProgressChanged)
        {
            if (!InternetIsAvailable())
                onConnectionFailed();

            DownloadString(url,
                (content) =>
                {
                    List<Book> lb = BookParse(content);
                    if (lb != null)
                        onContentReady(lb[0]);
                    else
                        onConnectionFailed();
                },
                () =>
                {
                    onConnectionFailed();
                },
                (percentage) =>
                {
                    onProgressChanged(percentage);
                });
        }

        public void DownloadBookReviews(string url, Action<List<Review>> onContentReady, Action onConnectionFailed, Action<int> onProgressChanged)
        {
            if (!InternetIsAvailable())
                onConnectionFailed();

            DownloadString(url,
                (content) =>
                {
                    List<Review> lr = ReviewsParse(content);
                    if (lr != null)
                        onContentReady(lr);
                    else
                        onConnectionFailed();
                },
                () =>
                {
                    onConnectionFailed();
                },
                (percentage) =>
                {
                    onProgressChanged(percentage);
                });
        }

        public void SearchBook(string url, Action<List<Book>> onContentReady, Action onConnectionFailed, Action<int> onProgressChanged)
        {
            if (!InternetIsAvailable())
                onConnectionFailed();

            DownloadString(url,
                (content) =>
                {
                    List<Book> lb = BookParse(content);
                    if (lb != null)
                        onContentReady(lb);
                    else
                        onConnectionFailed();
                },
                () =>
                {
                    onConnectionFailed();
                },
                (percentage) =>
                {
                    onProgressChanged(percentage);
                });
        }

        public List<Book> BookParse(string xmlFile)
        {
            List<Book> bl = new List<Book>();
            
            XNamespace d = @"http://www.w3.org/2005/Atom";
            XNamespace db = @"http://www.douban.com/xmlns/";
            XNamespace gd = @"http://schemas.google.com/g/2005";
            XNamespace opensearch = @"http://a9.com/-/spec/opensearchrss/1.0/";

            XDocument doc = XDocument.Parse(xmlFile);
            foreach (XElement entry in doc.Descendants(d + "entry"))
            {
                Book book = new Book();
                foreach (XElement element in entry.Descendants(d + "title"))
                {
                    book.Title = element.Value;
                }

                foreach (XElement element in entry.Descendants(d + "summary"))
                {
                    book.Summary = element.Value;
                }

                foreach (XElement element in entry.Descendants(d + "link"))
                {
                    if (element.Attribute("rel").Value == "image")
                    {
                        book.Imageurl = element.Attribute("href").Value;
                    }
                    else if (element.Attribute("rel").Value == "alternate")
                    {
                        book.Weblink = element.Attribute("href").Value;
                    }
                }

                //对于加了冒号前缀的元素，使用下列代码
                foreach (XElement element in entry.Descendants(db + "attribute"))
                {
                    switch (element.Attribute("name").Value)
                    {
                        case "isbn13":
                            book.Isbn = element.Value;
                            break;
                        case "pages":
                            book.Pages = element.Value;
                            break;
                        case "author":
                            book.Author.Add(element.Value);
                            break;
                        case "price":
                            book.Price = element.Value;
                            break;
                        case "publisher":
                            book.Publisher = element.Value;
                            break;
                        case "pubdate":
                            book.Pubdate = element.Value;
                            break;
                        case "author-intro":
                            book.AuthorIntro = element.Value;
                            break;
                        case "binding":
                            book.Binding = element.Value;
                            break;
                        case "translator":
                            book.Translator.Add(element.Value);
                            break;
                        case "subtitle":
                            book.Subtitle = element.Value;
                            break;
                    }
                }

                foreach (XElement element in entry.Descendants(gd + "rating"))
                {
                    book.Rating.Average= element.Attribute("average").Value;
                    book.Rating.RatersNum = element.Attribute("numRaters").Value;
                }

                foreach (XElement element in entry.Descendants(db + "tag"))
                {
                    BookTag tag = new BookTag();
                    tag.Tag = element.Attribute("name").Value;
                    tag.Count = element.Attribute("count").Value;
                    book.Tags.Add(tag);
                }

                bl.Add(book);
            }

            return bl;
        }

        public List<Review> ReviewsParse(string xmlFile)
        {
            List<Review> rl = new List<Review>();

            XNamespace d = @"http://www.w3.org/2005/Atom";
            XNamespace db = @"http://www.douban.com/xmlns/";
            XNamespace gd = @"http://schemas.google.com/g/2005";

            XDocument doc = XDocument.Parse(xmlFile);
            foreach (XElement entry in doc.Descendants(d + "entry"))
            {
                Review rw = new Review();
                foreach (XElement element in entry.Descendants(d + "title"))
                {
                    rw.Title = element.Value;
                }

                foreach (XElement element in entry.Descendants(d + "author"))
                {
                    foreach (XElement e in element.Descendants(d + "name"))
                    {
                        rw.Author = e.Value;
                    }
                }

                foreach (XElement element in entry.Descendants(d + "summary"))
                {
                    rw.Summary = element.Value;
                }

                foreach (XElement element in entry.Descendants(d + "published"))
                {
                    rw.Published = element.Value;
                }

                foreach (XElement element in entry.Descendants(d + "link"))
                {
                    if (element.Attribute("rel").Value == "alternate")
                    {
                        rw.WebLink = element.Attribute("href").Value;
                    }
                }

                foreach (XElement element in entry.Descendants(gd + "rating"))
                {
                    rw.Rating = element.Attribute("value").Value;
                }

                rl.Add(rw);
            }

            return rl;
        }
                
        public void GetBookBuyInfo(string url, Action<List<BuyLink>> onContentReady, Action onConnectionFailed, Action<int> onProgressChanged)
        {
            if (!InternetIsAvailable())
                onConnectionFailed();

            DownloadString(url,
                (content) =>
                {
                    List<BuyLink> buylinks = ParseBuylinks(content);
                    if (buylinks != null)
                        onContentReady(buylinks);
                    else
                        onConnectionFailed();
                },
                () =>
                {
                    onConnectionFailed();
                },
                (percentage) =>
                {
                    onProgressChanged(percentage);
                });
        }


        private List<BuyLink> ParseBuylinks(string buylink)
        {
            string re1 = "(<table class=)";	// content header
            string re2 = "(.*?)";	// Non-greedy match on filler
            string re3 = "(</table>)";	// tail
            List<BuyLink> buylinks = null;

            Regex r = new Regex(re1 + re2 + re3, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = r.Match(buylink);
            if (m.Success)
            {
                String content = m.Groups[2].ToString();
                List<string> providers = GetProviders(content);
                List<string> prices = GetPrices(content);
                if (providers.Count == prices.Count)
                {
                    buylinks = new List<BuyLink>();
                    for (int i = 0; i < providers.Count; i++)
                    {
                        buylinks.Add(new BuyLink { Provider = providers[i], Price = prices[i] });
                    }
                }
            }
            return buylinks;
        }

        private List<string> GetProviders(String content)
        {
            string dummy1 = "(;\">)";	// content header
            string bs = "([\u4e00-\u9fa5 | \\d*]*)";
            string dummy2 = "(</a>)";	// tail

            Regex rs = new Regex(dummy1 + bs + dummy2, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
            Match ms = rs.Match(content);
            List<string> providers = new List<string>();
            while (ms.Success)
            {
                string provider = ms.Groups[2].ToString();
                providers.Add(provider);
                ms = ms.NextMatch();
            }
            return providers;
        }

        private List<string> GetPrices(String content)
        {
            string dummy1 = "(href=\"[a-zA-z]+://[^\\s]*\">)";	// content header
            string bs = "(\\d*\\.\\d*)";
            string dummy2 = "(</a>)";	// tail

            Regex rs = new Regex(dummy1 + bs + dummy2, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match ms = rs.Match(content);
            List<string> prices = new List<string>();
            while (ms.Success)
            {
                string price = ms.Groups[2].ToString();
                prices.Add(price);
                ms = ms.NextMatch();
            }
            return prices;
        }


        public void DownloadBookReviewDetail(string url, Action<string> onContentReady, Action onConnectionFailed, Action<int> onProgressChanged)
        {
            if (!InternetIsAvailable())
                onConnectionFailed();

            DownloadString(url,
                (content) =>
                {
                    string processedHtml = ParseBookReviewDetail(content);
                    if (processedHtml != "")
                        onContentReady(processedHtml);
                    else
                        onConnectionFailed();
                },
                () =>
                {
                    onConnectionFailed();
                },
                (percentage) =>
                {
                    onProgressChanged(percentage);
                });
        }

        

        private bool IsThemeDark()
        {
            Visibility darkBackgroundVisibility = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"];
            if (darkBackgroundVisibility == Visibility.Visible)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string ParseBookReviewDetail(string rawHtml)
        {
            string processedHtml = "";

            string re1 = "(<span property=\"v:description\">)";    // content header
            string re2 = "(.*?)";    // Non-greedy match on filler
            string re3 = "(</span>)";    // tail

            Regex r = new Regex(re1 + re2 + re3, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = r.Match(rawHtml);
            if (m.Success)
            {
                String rd = m.Groups[2].ToString();
                string html = "";
                if (IsThemeDark())
                {
                    html += "<html lang='zh-CN'><head><meta content=\"initial-scale=1.0, user-scalable=no;\" /></head><body bgcolor=\"#000000\">";
                    html += "<p><font color=\"#FFFFFF\" size=\"3\">";
                }
                else
                {
                    html += "<html lang='zh-CN'><head><meta content=\"initial-scale=1.0, user-scalable=no;\" /></head><body bgcolor=\"#FFFFFF\">";
                    html += "<p><font color=\"#000000\" size=\"3\">";
                }
                
                html += rd;
                html += "</font></p></body></html>";
                processedHtml = ConvertExtendedAscii(html);
            }
            return processedHtml;
        }

        public static string ConvertExtendedAscii(string rawHtml)
        {
            string str = "";
            char c;
            for (int i = 0; i < rawHtml.Length; i++)
            {
                c = rawHtml[i];
                if (Convert.ToInt32(c) > 127)
                {
                    str += ("&#" + Convert.ToInt32(c) + ";");
                }
                else
                {
                    str += c.ToString();
                }
            }
            return str;
        }

        public static bool InternetIsAvailable()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return false;
            }
            return true;
        }

        public bool InternetIsAvailableNotify()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show(@"请检查网络连接。", "错误", MessageBoxButton.OK);
                return false;
            }
            return true;
        }

        private static void ShowUnexpectedError()
        {
            MessageBox.Show(@"We're sorry, but an unexpected connection error occurred. Please try again later.", "Oops!", MessageBoxButton.OK);
        }
    }
}
