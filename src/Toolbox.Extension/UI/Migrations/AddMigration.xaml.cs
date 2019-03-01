using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;
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

            viewModel.CloseAction = Close;

            DataContext = viewModel;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
