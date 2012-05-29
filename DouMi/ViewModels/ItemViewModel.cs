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
    public class ItemViewModel : INotifyPropertyChanged
    {
        public Book TheBook { get; private set; }

        public ItemViewModel(Book book)
        {
            TheBook = new Book();
            TheBook = book;
        }

        public void UpdateViewModel(Book book)
        {
            TheBook = book;
        }

        public ItemViewModel()
        {
            TheBook = new Book();
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

        public string Info
        {
            get 
            {
                string info = "";
                if (Author != "")
                    info += Author + " / ";
                if (Translator != "")
                    info += Translator + " / ";
                if (TheBook.Publisher != "")
                    info += TheBook.Publisher + " / ";
                if (TheBook.Pubdate != "")
                    info += TheBook.Pubdate + " / ";
                if (TheBook.Price != "")
                    info += TheBook.Price + " / ";
                if (TheBook.Rating.Average != "" && TheBook.Rating.Average != "0" && TheBook.Rating.RatersNum != "")
                    info += TheBook.Rating.Average + "(" + TheBook.Rating.RatersNum + "人评价)";
                return info;
            }
        }

        public  string CoverUrl
        {
            get { return TheBook.Imageurl; }
        }

        public string Price
        {
            get { return TheBook.Price; }
        }

        public string Isbn
        {
            get { return TheBook.Isbn; }
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