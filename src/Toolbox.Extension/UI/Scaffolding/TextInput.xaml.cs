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
            set
            {
                SetValue(ValueProperty, value);
                if (IsPassword && passwordBoxElement.Password != value)
                {
                    passwordBoxElement.Password = value;
                }
            }
        }

        public static readonly DependencyProperty IsPasswordProperty = DependencyProperty.Register(
            nameof(IsPassword), typeof(bool), typeof(TextInput), new FrameworkPropertyMetadata(false)
        );

        public bool IsPassword
        {
            get { return (bool)GetValue(IsPasswordProperty); }
            set { SetValue(IsPasswordProperty, value); }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (IsPassword)
            {
                this.Value = passwordBoxElement.Password;
            }
        }
    }
}
