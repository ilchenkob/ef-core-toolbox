using System.Windows;
using System.Windows.Controls;

namespace Toolbox.Extension.UI.Scaffolding
{
    /// <summary>
    /// Interaction logic for TextInput.xaml
    /// </summary>
    public partial class TextInput : UserControl
    {
        public TextInput()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title), typeof(string), typeof(TextInput), new FrameworkPropertyMetadata(string.Empty)
        );

        public string Title
        {
            get { return GetValue(TitleProperty)?.ToString(); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(string), typeof(TextInput), new FrameworkPropertyMetadata(string.Empty)
        );

        public string Value
        {
            get { return GetValue(ValueProperty)?.ToString(); }
            set { SetValue(ValueProperty, value); }
        }
    }
}
