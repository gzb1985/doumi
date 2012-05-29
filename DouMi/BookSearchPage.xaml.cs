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

namespace DouMi
{
    public partial class BookSearchPage : PhoneApplicationPage
    {
        string camIsbn = "";

        public BookSearchPage()
        {
            InitializeComponent();
            
            this.Loaded += new RoutedEventHandler(BookSearchPage_Loaded);
        }

        private void BookSearchPage_Loaded(object sender, RoutedEventArgs e)
        {
            txtSearchKey.Focus();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (camIsbn != "")
            {
                string temp = camIsbn;
                camIsbn = "";
                NavigationService.Navigate(new Uri("/BookDetailPanoramaPage.xaml?isbn=" + temp, UriKind.Relative));
            }
        }

        private void txtSearchKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && txtSearchKey.Text != "")
            {
                this.Focus();
                NavigationService.Navigate(new Uri("/BookSearchResultPage.xaml?key=" + txtSearchKey.Text, UriKind.Relative));
            }
        }

        private void txtSearchKey_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void BeginSearch_Click(object sender, EventArgs e)
        {
            if (txtSearchKey.Text != "")
            {
                this.Focus();
                NavigationService.Navigate(new Uri("/BookSearchResultPage.xaml?key=" + txtSearchKey.Text, UriKind.Relative));
            }
        }

        private void BarcodeScan_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/BarcodeScan/BarCode.xaml", UriKind.Relative));
        }

    }
}