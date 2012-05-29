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
    public class BuyLinkViewModel : INotifyPropertyChanged
    {
        public BuyLink TheBuyInfo = null;
        public BuyLinkViewModel(BuyLink bi)
        {
            TheBuyInfo = new BuyLink();
            TheBuyInfo = bi;
        }

        public void UpdateViewModel(BuyLink bi)
        {
            TheBuyInfo = bi;
        }

        public BuyLinkViewModel()
        {
            TheBuyInfo = new BuyLink();
            
        }

        /// <summary>
        /// 示例 ViewModel 属性；此属性在视图中用于使用绑定显示它的值。
        /// </summary>
        /// <returns></returns>
        public string Provider
        {
            get
            {
                return TheBuyInfo.Provider;
            }
            set
            {
                if (value != TheBuyInfo.Provider)
                {
                    TheBuyInfo.Provider = value;
                    NotifyPropertyChanged("Provider");
                }
            }
        }

        public string Price
        {
            get { return TheBuyInfo.Price; }
            set
            {
                if (value != TheBuyInfo.Price)
                {
                    TheBuyInfo.Price = value;
                    NotifyPropertyChanged("Price");
                }
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
