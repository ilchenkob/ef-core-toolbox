using System.Windows.Controls;

namespace Toolbox.Extension.UI.Scaffolding
{
    /// <summary>
    /// Interaction logic for OutputParams.xaml
    /// </summary>
    public partial class OutputParams : UserControl
    {
        public OutputParams()
        {
            InitializeComponent();
        }

        //private void BrowseFolder_Click(object sender, RoutedEventArgs e)
        //{
        //  var dialog = new CommonOpenFileDialog
        //  {
        //    IsFolderPicker = true,
        //    InitialDirectory = _viewModel.SelectedProjectPath,
        //    AddToMostRecentlyUsedList = false,
        //    DefaultDirectory = _viewModel.SelectedProjectPath,
        //    EnsurePathExists = true,
        //    Multiselect = false
        //  };

        //  if (dialog.ShowDialog(this) == CommonFileDialogResult.Ok)
        //  {
        //    var dtoFileName = Path.GetFileName(_viewModel.OutputFilePath);
        //    var selectedPath = dialog.FileName.Contains(_viewModel.SelectedProjectPath)
        //                        ? dialog.FileName.Substring(_viewModel.SelectedProjectPath.Length)
        //                        : dialog.FileName;
        //    _viewModel.OutputFilePath = Path.Combine(selectedPath, dtoFileName);
        //  }
        //}
    }
}
