using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
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
using Toolbox.Extension.Logic.Migrations.ViewModels;

namespace Toolbox.Extension.UI.Migrations
{
    /// <summary>
    /// Interaction logic for AddMigration.xaml
    /// </summary>
    public partial class AddMigration
    {
        public AddMigration(AddMigrationViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BrowseFolder_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddMigrationViewModel viewModel)
            {
                var dialog = new CommonOpenFileDialog
                {
                    IsFolderPicker = true,
                    // InitialDirectory = viewModel.Path,
                    AddToMostRecentlyUsedList = false,
                    // DefaultDirectory = viewModel.OutputPath,
                    EnsurePathExists = true,
                    Multiselect = false
                };

                if (dialog.ShowDialog(this) == CommonFileDialogResult.Ok)
                {
                    // viewModel.OutputPath = dialog.FileName;
                }
            }
        }
    }
}
