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
using System.Collections.Generic;

namespace DouMi
{
    public class MainPageLinksViewModel : INotifyPropertyChanged
    {
        public string iconPath = "";
        public string info = "";

        public MainPageLinksViewModel(string iconPath, string info)
        {
            this.iconPath = iconPath;
            this.info = info;
        }

        public string IconPath
        {
            get
            {
                return iconPath;
            }
        }

        public string Info
        {
            get { return info; }
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
