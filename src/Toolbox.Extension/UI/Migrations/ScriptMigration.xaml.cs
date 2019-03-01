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
using Toolbox.Extension.Logic.Scaffolding.ViewModels.TreeNodes;
using Toolbox.Extension.Logic.ViewModels.TreeNodes;

namespace Toolbox.Extension.UI.Migrations
{
    /// <summary>
    /// Interaction logic for ScriptMigration.xaml
    /// </summary>
    public partial class ScriptMigration
    {
        public ScriptMigration(ScriptMigrationViewModel viewModel)
        {
            InitializeComponent();

            viewModel.CloseAction = Close;

            DataContext = viewModel;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TreeCheckBox_Checked(object sender, RoutedEventArgs e) => setNodeState(sender, true);

        private void TreeCheckBox_Unchecked(object sender, RoutedEventArgs e) => setNodeState(sender, false);

        private void setNodeState(object sender, bool isChecked)
        {
            if (sender is CheckBox checkBox)
            {
                if (checkBox.DataContext is NodeViewModel vm)
                {
                    vm.ChangeState(isChecked);
                }
            }
        }
    }
}
