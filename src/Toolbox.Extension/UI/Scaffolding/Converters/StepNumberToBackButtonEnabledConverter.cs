using System;
using System.Globalization;
using System.Windows.Data;

namespace Toolbox.Extension.UI.Scaffolding.Converters
{
    public class StepNumberToBackButtonEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.TryParse(value?.ToString(), out int stepNumber))
            {
                return stepNumber > 0;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
