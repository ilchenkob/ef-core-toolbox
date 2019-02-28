using System;
using System.Globalization;
using System.Windows.Data;

namespace Toolbox.Extension.UI.Migrations.Converters
{
    public class SelectedContextToInputEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strValue = value?.ToString() ?? string.Empty;
            return !string.IsNullOrWhiteSpace(strValue) && !strValue.StartsWith("<");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
