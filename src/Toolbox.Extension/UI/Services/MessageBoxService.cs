using System;
using System.Threading.Tasks;

namespace Toolbox.Extension.UI.Services
{
    public interface IMessageBoxTrigger
    {
        Func<string, Task> ShowInfoMessageFunc { get; set; }

        Func<string, Task> ShowErrorMessageFunc { get; set; }

        Func<string, Task> ShowWarningMessageFunc { get; set; }
    }

    public interface IMessageBoxService
    {
        Task ShowInfoMessage(string message);

        Task ShowErrorMessage(string message);

        Task ShowWarningMessage(string message);
    }

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
