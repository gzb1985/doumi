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
using Microsoft.Phone.Tasks;

namespace DouMi
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
        }

        private void Email_Click(object sender, EventArgs e)
        {
            EmailComposeTask email = new EmailComposeTask();
            email.Subject = "豆米 v1.1.0.0 Feedback";
            email.To = "gzb1985@gmail.com";
            email.Show();
        }

        private void Website_Click(object sender, EventArgs e)
        {
            WebBrowserTask web = new WebBrowserTask();
            web.Uri = new Uri("http://www.cnblogs.com/gzb1985/", UriKind.Absolute);
            web.Show();
        }

        private void Review_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }
    }
}