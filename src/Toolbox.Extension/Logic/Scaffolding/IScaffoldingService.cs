using Toolbox.Extension.Logic.Scaffolding.Models;

namespace Toolbox.Extension.Logic.Scaffolding
{
    public interface IScaffoldingService
    {
        bool ScaffoldDatabase(ScaffoldingExecutorParams scaffoldingParams);
    }
}
