using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Toolbox.Extension.UI.Controls
{
    /// <summary>
    /// Interaction logic for ComboBoxItem.xaml
    /// </summary>
    public partial class ComboBoxItem : UserControl
    {
        public ComboBoxItem()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title), typeof(string), typeof(ComboBoxItem), new FrameworkPropertyMetadata(string.Empty)
        );

        public string Title
        {
            get { return GetValue(TitleProperty)?.ToString(); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            nameof(SelectedItem), typeof(string), typeof(ComboBoxItem), new FrameworkPropertyMetadata(string.Empty)
        );

        public string SelectedItem
        {
            get { return GetValue(SelectedItemProperty)?.ToString(); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            nameof(Items), typeof(ObservableCollection<string>), typeof(ComboBoxItem), new FrameworkPropertyMetadata(defaultValue: null)
        );

        public ObservableCollection<string> Items
        {
            get { return (ObservableCollection<string>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }
    }
}
