﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Toolbox.Extension.UI.Scaffolding.Converters
{
    public class TextInputTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var currentValue = System.Convert.ToBoolean(value);
            var paramValue = System.Convert.ToBoolean(parameter);

            return currentValue == paramValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
