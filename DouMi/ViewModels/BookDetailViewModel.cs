using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BookInfo;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DouMi
{
    public class BookDetailViewModel : INotifyPropertyChanged
    {
        public Book TheBook { get; private set; }
        public ObservableCollection<ReviewViewModel> Reviews { get; private set; }
        public ObservableCollection<BuyLinkViewModel> BuyLinks { get; private set; }

        public bool isLoaded = false;

        public BookDetailViewModel(Book book)
        {
            this.Reviews = new ObservableCollection<ReviewViewModel>();
            this.BuyLinks = new ObservableCollection<BuyLinkViewModel>();
            TheBook = new Book();
            TheBook = book;
        }

        public bool IsLoaded
        {
            get { return isLoaded; }
        }

        public void UpdateViewModel(Book book)
        {
            TheBook = book;
            isLoaded = true;
        }

        public BookDetailViewModel()
        {
            this.Reviews = new ObservableCollection<ReviewViewModel>();
            this.BuyLinks = new ObservableCollection<BuyLinkViewModel>();
            TheBook = new Book();
        }

        public void LoadReviews(List<Review> reviews)
        {
            Reviews.Clear();
            foreach (Review rw in reviews)
            {
                Reviews.Add(new ReviewViewModel(rw));
            }
        }

        public void AppendReviews(List<Review> reviews)
        {
            foreach (Review rw in reviews)
            {
                Reviews.Add(new ReviewViewModel(rw));
            }
        }

        public void LoadBuyInfo(List<BuyLink> buylinks)
        {
            BuyLinks.Clear();
            foreach (BuyLink bi in buylinks)
            {
                BuyLinks.Add(new BuyLinkViewModel(bi));
            }
        }

        /// <summary>
        /// 示例 ViewModel 属性；此属性在视图中用于使用绑定显示它的值。
        /// </summary>
        /// <returns></returns>
        public string Title
        {
            get
            {
                return TheBook.Title;
            }
            set
            {
                if (value != TheBook.Title)
                {
                    TheBook.Title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        public string Author
        {
            get
            {
                string auList = "";
                for (int i = 0; i < TheBook.Author.Count; i++)
                {
                    auList += TheBook.Author[i];
                    if (i != (TheBook.Author.Count - 1))
                        auList += " / ";
                }
                return auList;
            }
        }

        public string AuthorIntro
        {
            get { return TheBook.AuthorIntro; }
        }

        public string Translator
        {
            get
            {
                string trList = "";
                for (int i = 0; i < TheBook.Translator.Count; i++)
                {
                    trList += TheBook.Translator[i];
                    if (i != (TheBook.Translator.Count - 1))
                        trList += " / ";
                }
                return trList;
            }
        }

        /// <summary>
        /// 示例 ViewModel 属性；此属性在视图中用于使用绑定显示它的值。
        /// </summary>
        /// <returns></returns>
        public string CoverUrl
        {
            get { return TheBook.Imageurl; }
        }

        public string Summary
        {
            get { return TheBook.Summary; }
        }

        public string Isbn
        {
            get { return TheBook.Isbn; }
        }

        public string Weblink
        {
            get { return TheBook.Weblink; }
        }

        public string Pages
        {
            get { return TheBook.Pages; }
        }

        public string Price
        {
            get { return TheBook.Price; }
        }

        public string Publisher
        {
            get { return TheBook.Publisher; }
        }

        public string Pubdate
        {
            get { return TheBook.Pubdate; }
        }

        public string RatingAverage
        {
            get
            {
                if (TheBook.Rating.Average != "" && TheBook.Rating.Average != "0")
                    return TheBook.Rating.Average;
                else
                    return "";
            }
        }

        public string Rating
        {
            get
            {
                string rating = "";
                rating += TheBook.Rating.Average;
                rating += "(" + TheBook.Rating.RatersNum + "人评价)";
                return rating;
            }
        }

        public string BookBinding
        {
            get { return TheBook.Binding; }
        }

        public string Tags
        {
            get
            {
                string tags = "";
                foreach (BookTag tag in TheBook.Tags)
                {
                    tags += tag.Tag;
                    tags += "(" + tag.Count + ")";
                    tags += "、";
                }
                return tags;
            }
        }

        public string BasicInfoTitle
        {
            get { return "书名: " + Title; }
        }

        public string BasicInfoAuthor
        {
            get { return "作者: " + Author; }
        }

        public string BasicInfoTranslator
        {
            get { return "译者: " + Translator; }
        }

        public string BasicInfoPublisher
        {
            get { return "出版社: " + Publisher; }
        }

        public string BasicInfoPubdate
        {
            get { return "出版年: " + Pubdate; }
        }

        public string BasicInfoPages
        {
            get { return "页数: " + Pages; }
        }

        public string BasicInfoPrice
        {
            get { return "定价: " + Price; }
        }

        public string BasicInfoBinding
        {
            get { return "装帧: " + BookBinding; }
        }

        public string BasicInfoISBN
        {
            get { return "ISBN: " + Isbn; }
        }

        public string BasicInfoRating
        {
            get { return "评分: " + Rating; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
