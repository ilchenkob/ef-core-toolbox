namespace Migrator.Logic.Models
{
    public static class CommandType
    {
        public const string AddMigration = "add";

        public const string ScriptMigration = "script";

        public const string FindDbContextSubtypes = "finddbcontext";

        public const string FindMigrationSubtypes = "findmigration";
    }
}
