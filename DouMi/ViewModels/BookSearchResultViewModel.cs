using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using BookInfo;
using System.Collections.ObjectModel;


namespace DouMi
{
    public class BookSearchResultViewModel : INotifyPropertyChanged
    {
        public BookSearchResultViewModel()
        {
            this.Items = new ObservableCollection<ItemViewModel>();
        }

        /// <summary>
        /// ItemViewModel 对象的集合。
        /// </summary>
        public ObservableCollection<ItemViewModel> Items { get; private set; }

        public void LoadBookSearchResults(List<Book> books)
        {
            this.Items.Clear();
            AppendBookSearchResults(books);
        }

        public void AppendBookSearchResults(List<Book> books)
        {
            foreach (Book book in books)
            {
                this.Items.Add(new ItemViewModel(book));
            }
        }

        public bool IsItemsEmpty()
        {
            return Items.Count == 0;
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