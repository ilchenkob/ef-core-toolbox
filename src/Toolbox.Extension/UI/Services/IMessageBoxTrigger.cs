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
}
