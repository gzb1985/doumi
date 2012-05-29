using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Text.RegularExpressions;

namespace BookInfo
{
    
    public class BookUrl
    {

        public static string apiKey = @"0669e688d846e260133db39a88d4a720";
        public static string searchBaseUrl = @"http://api.douban.com/book/subjects";
        public static string isbnBookBaseUrl = @"http://api.douban.com/book/subject/isbn/";


        static BookUrl instance = null;

        public BookUrl()
        {
        }

        public static BookUrl Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BookUrl();
                }
                return instance;
            }
        }

        public string ConstructBookUrl(string isbn)
        {
            string bookUrl = isbnBookBaseUrl;
            bookUrl += isbn;
            bookUrl += "?";
            bookUrl += "apikey=" + apiKey;
            return bookUrl;
        }

        public string ConstructBookReviewsUrl(string isbn, int startIndex, int maxResults)
        {
            string bookReviewsUrl = isbnBookBaseUrl;
            bookReviewsUrl += isbn;
            bookReviewsUrl += "/reviews?";
            bookReviewsUrl += "&start-index=" + startIndex.ToString();
            bookReviewsUrl += "&max-results=" + maxResults.ToString();
            bookReviewsUrl += "&apikey=" + apiKey;
            return bookReviewsUrl;
        }

        public string ConstructBookSearchUrl(string searchKey, int startIndex, int maxResults)
        {
            string searchUrl = searchBaseUrl;
            searchUrl += "?";
            searchUrl += "q=" + searchKey;
            searchUrl += "&start-index=" + startIndex.ToString();
            searchUrl += "&max-results=" + maxResults.ToString();
            searchUrl += "&apikey=" + apiKey;
            return searchUrl;
        }

    }

    class BookSearchUrl : BookUrl
    {
        private int inc_results = 10;
        private int search_times = 0;

        private string searchKey = "";

        public BookSearchUrl(string searchKey)
        {
            this.searchKey = searchKey;
        }

        public string GetBookSearchUrl()
        {
            string searchUrl = "";
            searchUrl += searchBaseUrl;
            searchUrl += "?";
            searchUrl += "q=" + searchKey;
            searchUrl += "&start-index=" + (inc_results * search_times + 1).ToString();
            searchUrl += "&max-results=" + ((search_times+1) * inc_results).ToString();
            search_times++;
            searchUrl += "&apikey=" + apiKey;
            return searchUrl;
        }
    }

    
}
