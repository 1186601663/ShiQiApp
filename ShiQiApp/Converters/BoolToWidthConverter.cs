using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ShiQiApp.Converters
{
    public class BoolToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double expandedWidth = 200;
            double collapsedWidth = 50;

            if (parameter?.ToString().Split(',') is string[] parts && parts.Length == 2)
            {
                _ = double.TryParse(parts[0], out expandedWidth);
                _ = double.TryParse(parts[1], out collapsedWidth);
            }

            return (value as bool?) == true ? expandedWidth : collapsedWidth;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
