using System.Windows;
using System.Windows.Controls;

namespace Toolbox.Extension.UI.Scaffolding
{
    /// <summary>
    /// Interaction logic for WizardSteps.xaml
    /// </summary>
    public partial class WizardSteps : UserControl
    {
        public WizardSteps()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty CurrentStepProperty = DependencyProperty.Register(
            nameof(CurrentStep), typeof(int), typeof(WizardSteps), new FrameworkPropertyMetadata(0)
        );

        public int CurrentStep
        {
            get { return (int)GetValue(CurrentStepProperty); }
            set { SetValue(CurrentStepProperty, value); }
        }
    }
}
