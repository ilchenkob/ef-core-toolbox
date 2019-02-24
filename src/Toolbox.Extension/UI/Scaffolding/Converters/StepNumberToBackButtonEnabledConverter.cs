using System;
using System.Globalization;
using System.Windows.Data;
using Toolbox.Extension.Logic.Scaffolding.ViewModels;

namespace Toolbox.Extension.UI.Scaffolding.Converters
{
    public class StepNumberToBackButtonEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is WizardState state)
            {
                return state.CurrentStep > 0 && !state.IsLoading;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
