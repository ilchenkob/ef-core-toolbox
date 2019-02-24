using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;
using System.Windows.Controls;
using Toolbox.Extension.Logic.Scaffolding.ViewModels;

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

        private void BrowseFolder_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OutputParamsViewModel viewModel)
            {
                var dialog = new CommonOpenFileDialog
                {
                    IsFolderPicker = true,
                    InitialDirectory = viewModel.OutputPath,
                    AddToMostRecentlyUsedList = false,
                    DefaultDirectory = viewModel.OutputPath,
                    EnsurePathExists = true,
                    Multiselect = false
                };

                if (dialog.ShowDialog(ScaffoldingWizard.Instance) == CommonFileDialogResult.Ok)
                {
                    viewModel.OutputPath = dialog.FileName;
                }
            }
        }
    }
}
