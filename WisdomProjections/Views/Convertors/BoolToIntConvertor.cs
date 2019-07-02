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
    public class BoolToIntConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var w = bool.Parse(value.ToString());
            return w?1:0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var w = int.Parse(value.ToString());
            return w==1;
        }
    }
}
