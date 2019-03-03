using Migrator.Logic.Models;
using System.Collections.Generic;

namespace Toolbox.Extension.Logic.Migrations
{
    public interface IMigrationService
    {
        int AddMigration(AddMigrationParams commandParams);

        int ScriptMigration(ScriptMigrationParams commandParams);

        List<string> GetDbContextNames(FindDbContextSubtypeParams commandParams);

        Dictionary<string, List<string>> GetMigrationNames(FindMigrationSubtypeParams commandParams);
    }
}
