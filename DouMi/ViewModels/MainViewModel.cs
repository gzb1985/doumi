using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;


namespace DouMi
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<MainPageLinksViewModel> Items { get; private set; }

        public MainViewModel()
        {
            this.Items = new ObservableCollection<MainPageLinksViewModel>();
        }

        /// <summary>
        /// ItemViewModel 对象的集合。
        /// </summary>

        private string _sampleProperty = "示例运行时属性值";
        /// <summary>
        /// 示例 ViewModel 属性；此属性在视图中用于使用绑定显示它的值
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建一些 ItemViewModel 对象并将其添加到 Items 集合中。
        /// </summary>
        public void LoadData()
        {
            // 示例数据；替换为实际数据
            this.Items.Add(new MainPageLinksViewModel("/Images/Search.png", "搜索图书"));
            this.Items.Add(new MainPageLinksViewModel("/Images/Info.png", "关于..."));

            this.IsDataLoaded = true;
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