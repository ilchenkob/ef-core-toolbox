using System;
using System.Globalization;
using System.Windows.Data;

namespace Toolbox.Extension.UI.Scaffolding.Converters
{
    public class AuthTypeToRadioStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (bool.TryParse(value?.ToString(), out bool boolValue) &&
                bool.TryParse(parameter?.ToString(), out bool expectedBoolValue))
            {
                return boolValue == expectedBoolValue;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}
