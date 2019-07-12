using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WisdomProjections.Views.Convertors
{
    public class DivideConvertor : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            
                var w = double.Parse(values[values.Length-1].ToString());
                return w / double.Parse(parameter.ToString());
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var w = double.Parse(value.ToString());
            return new object[]{ w * double.Parse(parameter.ToString())};
        }
    }
}
