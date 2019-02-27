using System.Threading.Tasks;

namespace Toolbox.Extension.UI.Services
{
    public interface IMessageBoxService
    {
        Task ShowInfoMessage(string message);

        Task ShowErrorMessage(string message);

        Task ShowWarningMessage(string message);
    }
}
