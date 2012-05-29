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
using WikiDev.Core;
using BookInfo;
using com.google.zxing;
using System.Windows.Navigation;

namespace DouMi
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 构造函数
        public MainPage()
        {
            InitializeComponent();

            // 将 listbox 控件的数据上下文设置为示例数据
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            System.Threading.Thread.Sleep(1500);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        // 为 ViewModel 项加载数据
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
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
                NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
            }
            MainPageLinksListBox.SelectedIndex = -1;
        }
        
    }
}