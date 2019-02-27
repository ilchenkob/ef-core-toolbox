using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace Toolbox.Extension.UI.Migrations.Converters
{
    public class ItemsCountToCOmboboxEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<string> collection)
                return collection.Count > 0;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
