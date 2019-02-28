using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace Toolbox.Extension.UI.Controls
{
    /// <summary>
    /// Interaction logic for DirectoryInput.xaml
    /// </summary>
    public partial class DirectoryInput : UserControl
    {
        public DirectoryInput()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty PathProperty = DependencyProperty.Register(
            nameof(Path), typeof(string), typeof(DirectoryInput), new FrameworkPropertyMetadata(string.Empty)
        );

        public string Path
        {
            get { return GetValue(PathProperty)?.ToString(); }
            set { SetValue(PathProperty, value); }
        }

        private void BrowseFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = Path,
                AddToMostRecentlyUsedList = false,
                DefaultDirectory = Path,
                EnsurePathExists = true,
                Multiselect = false
            };

            var parentWindow = Window.GetWindow(this);
            if (dialog.ShowDialog(parentWindow) == CommonFileDialogResult.Ok)
            {
                Path = dialog.FileName;
            }
        }
    }
}
