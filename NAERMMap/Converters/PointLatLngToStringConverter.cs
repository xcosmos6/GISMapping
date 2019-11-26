using MapService.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NAERMMap.Converters
{
    public class PointLatLngToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var lc = (LatLngPoint)value;
            if (lc != null)
            {
                return "Lng: " + lc.Lng.ToString() + "\n" + "Lat: " + lc.Lat.ToString();
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
