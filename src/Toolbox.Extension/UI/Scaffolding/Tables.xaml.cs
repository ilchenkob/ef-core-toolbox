using System.Windows;
using System.Windows.Controls;
using Toolbox.Extension.Logic.Scaffolding.ViewModels.TreeNodes;
using Toolbox.Extension.Logic.ViewModels.TreeNodes;

namespace Toolbox.Extension.UI.Scaffolding
{
    /// <summary>
    /// Interaction logic for Tables.xaml
    /// </summary>
    public partial class Tables : UserControl
    {
        public Tables()
        {
            InitializeComponent();
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
