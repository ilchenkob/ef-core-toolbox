namespace Migrator.Logic.Models
{
    public class AddMigrationParams : IMigratorParams
    {
        public string Command => CommandType.AddMigration;

        public string AssemblyFileName { get; set; }

        public string ContextNamespace { get; set; }

        public string DbContextTypeFullName { get; set; }

        public string MigrationName { get; set; }

        public string OutputDir { get; set; }

        public string ProjectDir { get; set; }

        public string SubNamespace { get; set; }

        public string ToArgumentString()
        {
            return $"{Command} \"{AssemblyFileName}\" \"{ContextNamespace}\" \"{DbContextTypeFullName}\" \"{MigrationName}\" \"{OutputDir}\" \"{ProjectDir}\" \"{SubNamespace}\"";
        }

        public static AddMigrationParams FromArgumentsArray(string[] args)
        {
            if (args == null || args.Length < 8)
                throw new System.ArgumentException($"Invalid arguments. Can not construct {nameof(AddMigrationParams)}");

            return new AddMigrationParams
            {
                AssemblyFileName      = args[1],
                ContextNamespace      = args[2],
                DbContextTypeFullName = args[3],
                MigrationName         = args[4],
                OutputDir             = args[5],
                ProjectDir            = args[6],
                SubNamespace          = args[7]
            };
        }
    }
}
