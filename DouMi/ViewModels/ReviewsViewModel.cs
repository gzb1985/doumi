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

namespace DouMi
{
    public class ReviewViewModel : INotifyPropertyChanged
    {
        public Review TheReview = null;
        public ReviewViewModel(Review rw)
        {
            TheReview = new Review();
            TheReview = rw;
        }

        public void UpdateViewModel(Review rw)
        {
            TheReview = rw;
        }

        public ReviewViewModel()
        {
            TheReview = new Review();
            
        }

        /// <summary>
        /// 示例 ViewModel 属性；此属性在视图中用于使用绑定显示它的值。
        /// </summary>
        /// <returns></returns>
        public string Title
        {
            get
            {
                return TheReview.Title;
            }
            set
            {
                if (value != TheReview.Title)
                {
                    TheReview.Title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        public string Author
        {
            get 
            {
                return TheReview.Author; 
            }
        }

        public string Summary
        {
            get { return TheReview.Summary; }
            set
            {
                if (value != TheReview.Summary)
                {
                    TheReview.Summary = value;
                    NotifyPropertyChanged("Summary");
                }
            }
        }

        public string Rating
        {
            get 
            {
                return "(" + TheReview.Rating + "分)"; 
            }
        }

        public string WebLink
        {
            get
            {
                return TheReview.WebLink;
            }
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