using System.Windows;
using System.Windows.Input;
using Toolbox.Extension.Logic.Scaffolding.ViewModels;

namespace Toolbox.Extension.UI.Scaffolding
{
    /// <summary>
    /// Interaction logic for ScaffoldingWizard.xaml
    /// </summary>
    public partial class ScaffoldingWizard
    {
        private readonly ScaffoldingWizardViewModel _viewModel;

        public ScaffoldingWizard(ScaffoldingWizardViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            _viewModel.CloseAction = () => Cancel_Click(null, null);

            DataContext = viewModel;

            KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Cancel_Click(null, null);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            _viewModel?.Dispose();
            Close();
        }
    }
}
