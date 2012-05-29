using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DouMi.Converters
{
    /// <summary>
    /// A type converter for visibility and boolean values.
    /// </summary>
    public class BasicInfoToVisibilityConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            string info = (string)value;
            info.Trim();
            return (info != "") ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;
            return (visibility == Visibility.Visible);
        }
    }
}