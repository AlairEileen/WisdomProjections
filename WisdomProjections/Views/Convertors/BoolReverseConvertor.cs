using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WisdomProjections.Views.Convertors
{
    public class BoolReverseConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var w = value != null && bool.Parse(value.ToString());
            return !w;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var w = value != null && bool.Parse(value.ToString());
            return !w;
        }
    }
}
