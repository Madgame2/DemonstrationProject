using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DemonstrationProject.Convertors
{
    public class DecemalToStringConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal d)
                return d.ToString("N2", culture); 

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string input = value?.ToString()?.Replace(" ", "") ?? "";

            if (decimal.TryParse(input, NumberStyles.Any, culture, out var result))
                return result;

            return DependencyProperty.UnsetValue;
        }
    }
}
