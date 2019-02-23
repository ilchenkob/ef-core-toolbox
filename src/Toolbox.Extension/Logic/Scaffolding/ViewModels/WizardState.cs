using System;

namespace Toolbox.Extension.Logic.Scaffolding.ViewModels
{
    public class WizardState
    {
        private Func<int> _currStep;
        private Func<bool> _isConnectionValid;
        private Func<bool> _isTablesValid;
        private Func<bool> _isOutputParamsValid;

        public WizardState(
            Func<int> currStep,
            Func<bool> isConnectionValid,
            Func<bool> isTablesValid,
            Func<bool> isOutputParamsValid)
        {
            _currStep = currStep;
            _isConnectionValid = isConnectionValid;
            _isTablesValid = isTablesValid;
            _isOutputParamsValid = isOutputParamsValid;
        }

        public int CurrentStep => _currStep();
        public bool IsConnectionValid => _isConnectionValid();
        public bool IsTablesValid => _isTablesValid();
        public bool IsOutputValid => _isOutputParamsValid();
    }
}
