namespace Migrator.Logic.Models
{
    public class ScriptMigrationParams : IMigratorParams
    {
        public string Command => CommandType.ScriptMigration;

        public string AssemblyFileName { get; set; }

        public string MigrationName { get; set; }

        public string ToArgumentString()
        {
            return $"{Command} \"{AssemblyFileName}\" \"{MigrationName}\"";
        }

        public static ScriptMigrationParams FromArgumentsArray(string[] args)
        {
            if (args == null || args.Length < 3)
                throw new System.ArgumentException($"Invalid arguments. Can not construct {nameof(ScriptMigrationParams)}");

            return new ScriptMigrationParams
            {
                AssemblyFileName = args[1],
                MigrationName = args[2]
            };
        }
    }
}
