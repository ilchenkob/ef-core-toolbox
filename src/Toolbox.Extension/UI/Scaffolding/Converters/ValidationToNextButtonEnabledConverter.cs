using System;
using System.Globalization;
using System.Windows.Data;
using Toolbox.Extension.Logic.Scaffolding.ViewModels;

namespace Toolbox.Extension.UI.Scaffolding.Converters
{
    public class ValidationToNextButtonEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is WizardState state)
            {
                if (!state.IsLoading)
                {
                    switch (state.CurrentStep)
                    {
                        case 0:
                            return state.IsConnectionValid;
                        case 1:
                            return state.IsConnectionValid && state.IsTablesValid;
                        case 2:
                            return state.IsConnectionValid && state.IsTablesValid && state.IsOutputValid;
                        default:
                            return false;
                    }
                }
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
