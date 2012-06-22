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
using WebHelpers;
using BookInfo;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
using System.Collections;

namespace DouMi
{
    public partial class BookDetailPanoramaPage : PhoneApplicationPage
    {
        public BookDetailViewModel bookDetailViewModel = null;

        private ScrollViewer sv = null;
        private bool alreadyHookedScrollEvents = false;
        private bool isLoadingReviews = false;
        private bool needLoadMoreReviews = false;
        private bool isNoMoreReviews = false;

        private string Isbn = "";
        
        public BookDetailPanoramaPage()
        {
            bookDetailViewModel = new BookDetailViewModel();
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(BookDetailPanoramaPage_Loaded);
        }

        private void BookDetailPanoramaPage_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                initialScrollView();
            });
        }

        private void ReviewsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReviewsListBox.SelectedIndex == -1)
                return;
            int reviewIndex = ReviewsListBox.SelectedIndex;
            string title = bookDetailViewModel.Reviews[reviewIndex].Title;
            string author = bookDetailViewModel.Reviews[reviewIndex].Author;
            string webLink = bookDetailViewModel.Reviews[reviewIndex].WebLink;
            string rating = bookDetailViewModel.Reviews[reviewIndex].Rating;
            string bookTitle = bookDetailViewModel.Title;

            NavigationService.Navigate(new Uri("/BookReviewDetail.xaml?title=" + title + "&author=" + author + "&weblink=" + webLink + 
                "&rating=" + rating + "&booktitle=" + bookTitle, UriKind.Relative));

            ReviewsListBox.SelectedIndex = -1;
        }

        // 导航页面以将数据上下文设置为列表中的所选项时
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (bookDetailViewModel.IsLoaded)
                return;

            string isbn = "";
            
            if (NavigationContext.QueryString.TryGetValue("isbn", out isbn))
            {
                if (isbn != "")
                {
                    Isbn = isbn;
                    GetBookInfo(isbn);
                }
                else
                {
                    InvalidBook();
                }
            }

            string title = "";
            NavigationContext.QueryString.TryGetValue("title", out title);
            if (title != null && title != "")
            {
                DisplayBookTitle(title);
            }
            base.OnNavigatedTo(e);
        }

        private void InvalidBook()
        {
            Dispatcher.BeginInvoke(() =>
            {
                Book nullBook = new Book();
                bookDetailViewModel.UpdateViewModel(nullBook);//null book
                this.DataContext = bookDetailViewModel;
                noBasicInfo.Visibility = System.Windows.Visibility.Visible;
                noSummaryInfo.Visibility = System.Windows.Visibility.Visible;
                noReview.Visibility = System.Windows.Visibility.Visible;
                noBuyinfo.Visibility = System.Windows.Visibility.Visible;
            });
        }

        private void DisplayBookTitle(string title)
        {
            Dispatcher.BeginInvoke(() =>
            {
                bookTitle.Text = title;
                bookTitle.Visibility = System.Windows.Visibility.Visible;
            });
        }

        private void HideIntroTitles()
        {
            Dispatcher.BeginInvoke(() =>
            {
                authorIntro.Opacity = 0;
                contentIntro.Opacity = 0;
            });
        }

        private void DisplayIntroTitles()
        {
            Dispatcher.BeginInvoke(() =>
            {
                authorIntro.Opacity = 1;
                contentIntro.Opacity = 1;
            });
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            var lastPage = NavigationService.BackStack.FirstOrDefault();
            if (lastPage != null && lastPage.Source.ToString() == "/BarcodeScan/BarCode.xaml")
            {
                NavigationService.RemoveBackEntry();
            }

            base.OnNavigatingFrom(e);
        }

        private void GetBookInfo(string isbn)
        {
            string bookUrl = BookUrl.Instance.ConstructBookUrl(isbn);
            basicInfoProgressBar.IsIndeterminate = true;
            HideIntroTitles();
            WebHelper.Instance.DownloadBookInfo(bookUrl,
                    (book) =>
                    {
                        DisplayBookTitle(book.Title);
                        Dispatcher.BeginInvoke(() =>
                        {
                            bookDetailViewModel.UpdateViewModel(book);
                            this.DataContext = bookDetailViewModel;
                            if (bookDetailViewModel.Summary == "" && bookDetailViewModel.AuthorIntro == "")
                                noSummaryInfo.Visibility = System.Windows.Visibility.Visible;
                            DisplayIntroTitles();
                            GetBookReviews(isbn);
                        });
                        basicInfoProgressBar.IsIndeterminate = false;
                    },
                    () =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            InvalidBook();
                            DisplayIntroTitles();
                        });
                        basicInfoProgressBar.IsIndeterminate = false;
                    },
                    (percentage) =>
                    {
                    }
            );
        }

        private void GetBookReviews(string isbn)
        {
            string bookReviewsUrl = BookUrl.Instance.ConstructBookReviewsUrl(isbn, 1, 5);
            isLoadingReviews = true;
            reviewProgressBar.IsIndeterminate = true;
            WebHelper.Instance.DownloadBookReviews(bookReviewsUrl,
                    (rws) =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            bookDetailViewModel.LoadReviews(rws);
                            GetBookBuyInfo(isbn, bookDetailViewModel.Weblink);
                            if (bookDetailViewModel.Reviews.Count == 0)
                                noReview.Visibility = System.Windows.Visibility.Visible;
                        });
                        isLoadingReviews = false;
                        reviewProgressBar.IsIndeterminate = false;
                    },
                    () =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            if (bookDetailViewModel.Reviews.Count == 0)
                            {
                                noReview.Visibility = System.Windows.Visibility.Visible;
                            }
                            GetBookBuyInfo(isbn, bookDetailViewModel.Weblink);
                        });
                        isLoadingReviews = false;
                        reviewProgressBar.IsIndeterminate = false;
                    },
                    (percentage) =>
                    {
                    }
            );
        }

        private void GetMoreBookReviews(string url)
        {
            reviewProgressBar.IsIndeterminate = true;
            isLoadingReviews = true;
            WebHelper.Instance.DownloadBookReviews(url,
                    (rws) =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            if (rws.Count == 0)
                            {
                                isNoMoreReviews = true;
                                if (bookDetailViewModel.Reviews.Count == 0)
                                {
                                    noReview.Visibility = System.Windows.Visibility.Visible;
                                }
                            }
                            else
                            {
                                bookDetailViewModel.AppendReviews(rws);
                            }
                            reviewProgressBar.IsIndeterminate = false;
                        });
                        
                        isLoadingReviews = false;
                    },
                    () =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            reviewProgressBar.IsIndeterminate = false;
                        });
                        isLoadingReviews = false;
                    },
                    (percentage) =>
                    {
                    }
            );
        }

        private void GetBookBuyInfo(string isbn, string weblink)
        {
            string bookBuyLinkUrl = weblink + "/buylinks";
            buylinkProgressBar.IsIndeterminate = true;
            WebHelper.Instance.GetBookBuyInfo(bookBuyLinkUrl,
                    (buylinks) =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            bookDetailViewModel.LoadBuyInfo(buylinks);
                            if (bookDetailViewModel.BuyLinks.Count == 0)
                                noBuyinfo.Visibility = System.Windows.Visibility.Visible;
                        });
                        buylinkProgressBar.IsIndeterminate = false;
                    },
                    () =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            noBuyinfo.Visibility = System.Windows.Visibility.Visible;
                        });
                        buylinkProgressBar.IsIndeterminate = false;
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
            ReviewsListBox.AddHandler(ListBox.ManipulationCompletedEvent, (EventHandler<ManipulationCompletedEventArgs>)ReviewsListBox_ManipulationCompleted, true);
            ReviewsListBox.AddHandler(ListBox.ManipulationStartedEvent, (EventHandler<ManipulationStartedEventArgs>)ReviewsListBox_ManipulationStarted, true);
            sv = (ScrollViewer)FindElementRecursive(ReviewsListBox, typeof(ScrollViewer));

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

        private bool NeedLoadMoreReviews()
        {
            return needLoadMoreReviews && !isLoadingReviews && !isNoMoreReviews;
        }

        private void ReviewsListBox_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (NeedLoadMoreReviews())
            {
                needLoadMoreReviews = false;
                Dispatcher.BeginInvoke(() =>
                {
                    int start = bookDetailViewModel.Reviews.Count + 1;
                    int results = 5;
                    string bookReviewsUrl = BookUrl.Instance.ConstructBookReviewsUrl(Isbn, start, results);
                    GetMoreBookReviews(bookReviewsUrl);
                });
            }
        }

        private void ReviewsListBox_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            needLoadMoreReviews = false;
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
                needLoadMoreReviews = true;
            }
            if (e.NewState.Name == "NoVerticalCompression" || e.NewState.Name == "CompressionTop")
            {
            }
        }
    }
}