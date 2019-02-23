using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Toolbox.Extension.UI.Scaffolding.Converters
{
    public class StepNumberToWizardStepContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.TryParse(value?.ToString(), out int stepNumber) &&
                int.TryParse(parameter?.ToString(), out int expectedStepNumber))
            {
                return stepNumber == expectedStepNumber ? Visibility.Visible : Visibility.Hidden;
            }

            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
