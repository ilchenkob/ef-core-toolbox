using Migrator.Logic.Models;

namespace Migrator.Logic
{
    public interface IAddMigrationExecutor
    {
        int Run(AddMigratorParams migrationParams);
    }
}
