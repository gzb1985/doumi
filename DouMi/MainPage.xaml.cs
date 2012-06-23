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
using WebHelpers;
using BookInfo;
using com.google.zxing;
using System.Windows.Navigation;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Shell;
using DouMi.OAuth;
using System.Windows.Media.Imaging;


namespace DouMi
{
    public partial class MainPage : PhoneApplicationPage
    {
        private IsolatedStorageSettings userSettings = IsolatedStorageSettings.ApplicationSettings;

        private DoubanOAuth doubanOAuth = new DoubanOAuth();

        // 构造函数
        public MainPage()
        {
            InitializeComponent();

            // 将 listbox 控件的数据上下文设置为示例数据
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            System.Threading.Thread.Sleep(500);
        }

        

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("AuthDone"))
            {
                string done = PhoneApplicationService.Current.State["AuthDone"] as string;
                if (done == "Yes")
                {
                    PhoneApplicationService.Current.State.Remove("AuthDone");
                    doubanOAuth.GetAccessToken(
                        (accessToken, secret) =>
                        {
                            doubanOAuth.GetAuthUserInfo((username, iconurl) =>
                            {
                                UserName.Text = username;
                                Uri imgURI = new Uri(iconurl, UriKind.Absolute);
                                UserIcon.Source = new BitmapImage(imgURI);
                                MessageBox.Show(iconurl, "OK", MessageBoxButton.OK);
                            });
                        },
                        () =>
                        {
                        }
                    );
                }
            }
            base.OnNavigatedTo(e);
        }

        // 为 ViewModel 项加载数据
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }

            if (doubanOAuth.isAuthed)
            {
                string url = doubanOAuth.ReadFromIsolatedStorage("iconurl");
                if (url != "")
                {
                    Uri imgURI = new Uri(url, UriKind.Absolute);
                    UserIcon.Source = new BitmapImage(imgURI);
                }

                string username = doubanOAuth.ReadFromIsolatedStorage("username");
                if (username != "")
                {
                    UserName.Text = username;
                }
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BookSearchPage.xaml", UriKind.Relative));
        }

        private void MainPageLinksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainPageLinksListBox.SelectedIndex == -1)
                return;

            if (MainPageLinksListBox.SelectedIndex == 0)
            {
                NavigationService.Navigate(new Uri("/BookSearchPage.xaml", UriKind.Relative));
            }
            else if (MainPageLinksListBox.SelectedIndex == 1)
            {
                if (doubanOAuth.isAuthed)
                {
                    doubanOAuth.GetAuthUserInfo( (username, iconurl) =>
                        {
                            UserName.Text = username;
                            Uri imgURI = new Uri(iconurl, UriKind.Absolute);  
                            UserIcon.Source = new BitmapImage(imgURI);
                            //MessageBox.Show(iconurl, "OK", MessageBoxButton.OK);
                            MessageBox.Show(username + "已认证", "OK", MessageBoxButton.OK);
                        }
                    );
                }
                else
                {
                    doubanOAuth.GetRequestToken(
                        (requestToken, secret) =>
                        {
                            string authLink = DoubanOAuth.authorizationUri + requestToken;
                            NavigationService.Navigate(new Uri("/DoubanOAuthPage.xaml?authLink=" + authLink, UriKind.Relative));
                        },
                        () =>
                        {
                            //
                        }
                    );
                }
            }
            else if (MainPageLinksListBox.SelectedIndex == 2)
            {
                NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
            }
            MainPageLinksListBox.SelectedIndex = -1;
        }


        
    }
}