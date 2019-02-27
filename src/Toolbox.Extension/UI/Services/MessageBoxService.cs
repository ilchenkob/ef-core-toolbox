using System;
using System.Threading.Tasks;

namespace Toolbox.Extension.UI.Services
{
    public class MessageBoxService : IMessageBoxTrigger, IMessageBoxService
    {
        public Func<string, Task> ShowInfoMessageFunc { get; set; }
        public Func<string, Task> ShowErrorMessageFunc { get; set; }

        public Func<string, Task> ShowWarningMessageFunc { get; set; }

        public Task ShowErrorMessage(string message) => ShowErrorMessageFunc?.Invoke(message);

        public Task ShowInfoMessage(string message) => ShowInfoMessageFunc?.Invoke(message);

        public Task ShowWarningMessage(string message) => ShowWarningMessageFunc?.Invoke(message);
    }
}
