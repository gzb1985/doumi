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
using Microsoft.Phone.Shell;

namespace DouMi
{
    public partial class DoubanOAuthPage : PhoneApplicationPage
    {
        public DoubanOAuthPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string authLink = "";
            if (NavigationContext.QueryString.TryGetValue("authLink", out authLink) && authLink != "")
            {
                authPage.Navigate(new Uri(authLink, UriKind.Absolute));
            }
            base.OnNavigatedTo(e);
        }

        private void Done_Click(object sender, EventArgs e)
        {
            PhoneApplicationService.Current.State["AuthDone"] = "Yes";
            NavigationService.GoBack();
        }
    }
}