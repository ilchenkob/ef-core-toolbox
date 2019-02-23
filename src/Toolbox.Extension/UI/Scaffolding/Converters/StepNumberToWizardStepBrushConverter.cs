using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Toolbox.Extension.UI.Scaffolding.Converters
{
    public class StepNumberToWizardStepBrushConverter : IValueConverter
    {
        private readonly Brush SelectedStepBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        private readonly Brush NotSelectedStepBrush = new SolidColorBrush(Color.FromRgb(136, 136, 136));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.TryParse(value?.ToString(), out int stepNumber) &&
                int.TryParse(parameter?.ToString(), out int expectedStepNumber))
            {
                return stepNumber == expectedStepNumber ? SelectedStepBrush : NotSelectedStepBrush;
            }

            return NotSelectedStepBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
