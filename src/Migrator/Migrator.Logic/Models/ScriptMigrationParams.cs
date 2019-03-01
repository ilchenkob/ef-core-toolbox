namespace Migrator.Logic.Models
{
    public class ScriptMigrationParams : IMigratorParams
    {
        private const char _separator = ';';

        public const char MigrationNamesSplitter = '>';

        public string Command => CommandType.ScriptMigration;

        public string AssemblyFileName { get; set; }

        public string DbContextFullName { get; set; }

        public string OutputPath { get; set; }

        public string[] Migrations { get; set; }

        public string ToArgumentString()
        {
            return $"{Command} \"{AssemblyFileName}\" \"{DbContextFullName}\" \"{OutputPath}\" \"{string.Join(_separator.ToString(), Migrations)}\"";
        }

        public static ScriptMigrationParams FromArgumentsArray(string[] args)
        {
            if (args == null || args.Length < 5)
                throw new System.ArgumentException($"Invalid arguments. Can not construct {nameof(ScriptMigrationParams)}");

            return new ScriptMigrationParams
            {
                AssemblyFileName = args[1],
                DbContextFullName = args[2],
                OutputPath = args[3],
                Migrations = args[4].Split(_separator)
            };
        }
    }
}
