using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Toolbox.Extension.UI.Scaffolding.Converters
{
    public class StepNumberToOkButtonVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.TryParse(value?.ToString(), out int stepNumber))
            {
                return stepNumber == 2 ? Visibility.Visible : Visibility.Hidden;
            }

            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
