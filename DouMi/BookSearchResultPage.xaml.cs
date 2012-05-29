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
using System.Windows.Controls.Primitives;
using WikiDev.Core;
using BookInfo;
using System.Collections;

namespace DouMi
{
    public partial class BookSearchResultPage : PhoneApplicationPage
    {
        BookSearchResultViewModel bookSearchResultViewModel = null;
        string SearchKey = "";

        private ScrollViewer sv = null;
        private bool alreadyHookedScrollEvents = false;
        private bool isLoadingMoreData = false;
        private bool needLoadingMoreData = false;

        public BookSearchResultPage()
        {
            bookSearchResultViewModel = new BookSearchResultViewModel();
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(BookSearchResultPage_Loaded);
        }

        private void BookSearchResultPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = bookSearchResultViewModel;
            initialScrollView();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string key = "";
            if (NavigationContext.QueryString.TryGetValue("key", out key) && key != "" && bookSearchResultViewModel.IsItemsEmpty())
            {
                SearchKey = key;
                string bookSearchUrl = BookUrl.Instance.ConstructBookSearchUrl(SearchKey, 1, 10);
                SearchBook(bookSearchUrl);
                ApplicationTitle.Text = "豆米 | 搜索\"" + key + "\"得到的...";
            }
            base.OnNavigatedTo(e);
        }

        private void BookSearchResultListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BookSearchResultListBox.SelectedIndex == -1)
                return;

            NavigationService.Navigate(new Uri("/BookDetailPanoramaPage.xaml?isbn=" + bookSearchResultViewModel.Items[BookSearchResultListBox.SelectedIndex].Isbn, UriKind.Relative));
            BookSearchResultListBox.SelectedIndex = -1;
        }

        private void SearchBook(string url)
        {
            performanceProgressBar.IsIndeterminate = true;
            WebHelper.Instance.SearchBook(url,
                    (bookList) =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            performanceProgressBar.IsIndeterminate = false;
                            bookSearchResultViewModel.LoadBookSearchResults(bookList);
                        });
                    },
                    () =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            performanceProgressBar.IsIndeterminate = false;
                            if (NavigationService.CanGoBack)
                                NavigationService.GoBack();
                        });
                    },
                    (percentage) =>
                    {
                    }
            );
        }

        private void SearchMoreBook(string url)
        {
            performanceProgressBar.IsIndeterminate = true;
            WebHelper.Instance.SearchBook(url,
                    (bookList) =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            performanceProgressBar.IsIndeterminate = false;
                            bookSearchResultViewModel.AppendBookSearchResults(bookList);
                            isLoadingMoreData = false;
                        });
                    },
                    () =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            performanceProgressBar.IsIndeterminate = false;
                            isLoadingMoreData = false;
                        });
                    },
                    (percentage) =>
                    {
                    }
            );
        }

        private void initialScrollView()
        {
            if (alreadyHookedScrollEvents)
                return;
            alreadyHookedScrollEvents = true;
            BookSearchResultListBox.AddHandler(ListBox.ManipulationCompletedEvent, (EventHandler<ManipulationCompletedEventArgs>)LB_ManipulationCompleted, true);
            BookSearchResultListBox.AddHandler(ListBox.ManipulationStartedEvent, (EventHandler<ManipulationStartedEventArgs>)LB_ManipulationStarted, true);
            sv = (ScrollViewer)FindElementRecursive(BookSearchResultListBox, typeof(ScrollViewer));

            if (sv != null)
            {
                // Visual States are always on the first child of the control template 
                FrameworkElement element = VisualTreeHelper.GetChild(sv, 0) as FrameworkElement;
                if (element != null)
                {
                    VisualStateGroup vgroup = FindVisualState(element, "VerticalCompression");
                    if (vgroup != null)
                    {
                        vgroup.CurrentStateChanging += new EventHandler<VisualStateChangedEventArgs>(vgroup_CurrentStateChanging);
                    }
                }
            }
        }

        private void LB_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (needLoadingMoreData)
            {
                needLoadingMoreData = false;
                Dispatcher.BeginInvoke(() =>
                {
                    isLoadingMoreData = true;
                    int start = bookSearchResultViewModel.Items.Count + 1;
                    int results = 10;
                    string bookSearchUrl = BookUrl.Instance.ConstructBookSearchUrl(SearchKey, start, results);
                    SearchMoreBook(bookSearchUrl);
                });
            }
        }

        private void LB_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            needLoadingMoreData = false;
        }

        private UIElement FindElementRecursive(FrameworkElement parent, Type targetType)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            UIElement returnElement = null;
            if (childCount > 0)
            {
                for (int i = 0; i < childCount; i++)
                {
                    Object element = VisualTreeHelper.GetChild(parent, i);
                    if (element.GetType() == targetType)
                    {
                        return element as UIElement;
                    }
                    else
                    {
                        returnElement = FindElementRecursive(VisualTreeHelper.GetChild(parent, i) as FrameworkElement, targetType);
                    }
                }
            }
            return returnElement;
        }
        private VisualStateGroup FindVisualState(FrameworkElement element, string name)
        {
            if (element == null)
                return null;

            IList groups = VisualStateManager.GetVisualStateGroups(element);
            foreach (VisualStateGroup group in groups)
                if (group.Name == name)
                    return group;

            return null;
        }

        private void vgroup_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState.Name == "CompressionBottom")
            {
                if (!isLoadingMoreData)
                {
                    needLoadingMoreData = true;
                }
            }
            if (e.NewState.Name == "NoVerticalCompression" || e.NewState.Name == "CompressionTop")
            {
            }
        }
    }
}