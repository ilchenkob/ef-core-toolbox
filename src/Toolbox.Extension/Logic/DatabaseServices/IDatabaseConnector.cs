using System.Threading.Tasks;

namespace Toolbox.Extension.Logic.Scaffolding.DatabaseServices
{
    public interface IDatabaseConnector
    {
        Task<bool> TryConnect(string connectionString);
    }
}
