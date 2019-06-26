using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WisdomProjections.Views.Convertors
{
    public class HalfNumConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var w = double.Parse(value.ToString());
            return w / int.Parse(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var w = double.Parse(value.ToString());
            return w * int.Parse(parameter.ToString());
        }
    }
}
