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
using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using WikiDev.Core;
using System.Text.RegularExpressions;
using System.IO.IsolatedStorage;
using System.IO;
using System.Text;

namespace DouMi
{
    public partial class BookReviewDetail : PhoneApplicationPage
    {
        private bool isReviewDetailLoaded = false;

        public BookReviewDetail()
        {
            InitializeComponent();
        }

        // 导航页面以将数据上下文设置为列表中的所选项时
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (isReviewDetailLoaded)
                return;

            string title = "";
            string author = "";
            string weblink = "";
            string rating = "";
            string booktitle = "";

            NavigationContext.QueryString.TryGetValue("title", out title);
            NavigationContext.QueryString.TryGetValue("author", out author);
            NavigationContext.QueryString.TryGetValue("rating", out rating);
            NavigationContext.QueryString.TryGetValue("weblink", out weblink);
            NavigationContext.QueryString.TryGetValue("booktitle", out booktitle);

            ApplicationTitle.Text = "<<" + booktitle + ">>评论"; 
            PageTitle.Text = title;
            Author.Text = author;
            Rating.Text = rating;

            if (NavigationContext.QueryString.TryGetValue("weblink", out weblink))
            {
                GetBookReviewDetail(weblink);
            }
        }

        private void WebBrowserReview_LoadCompleted(object sender, NavigationEventArgs e) 
        {
            webBrowserReview.Opacity = 1;
        } 

        private void GetBookReviewDetail(string weblink)
        {
            reviewDetailProgressBar.IsIndeterminate = true;
            WebHelper.Instance.DownloadBookReviewDetail(weblink,
                    (content) =>
                    {
                        webBrowserReview.NavigateToString(content);
                        isReviewDetailLoaded = true;
                        reviewDetailProgressBar.IsIndeterminate = false;
                    },
                    () =>
                    {
                        reviewDetailProgressBar.IsIndeterminate = false;
                    },
                    (percentage) =>
                    {
                    }
            );
        }
    }
}