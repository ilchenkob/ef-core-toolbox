namespace Migrator.Logic.Models
{
    public class AddMigratorParams : MigratorParams
    {
        public override string Command => CommandType.AddMigration;

        public string AssemblyFileName { get; set; }

        public string ContextNamespace { get; set; }

        public string DbContextTypeFullName { get; set; }

        public string MigrationName { get; set; }

        public string OutputDir { get; set; }

        public string ProjectDir { get; set; }

        public string SubNamespace { get; set; }

        public override string ToArgumentString()
        {
            return $"{Command} \"{AssemblyFileName}\" \"{ContextNamespace}\" \"{DbContextTypeFullName}\" \"{MigrationName}\" \"{OutputDir}\" \"{ProjectDir}\" \"{SubNamespace}\"";
        }

        public static AddMigratorParams FromArgumentsArray(string[] args)
        {
            return new AddMigratorParams
            {
                AssemblyFileName = args[1],
                ContextNamespace = args[2],
                DbContextTypeFullName = args[3],
                MigrationName = args[4],
                OutputDir = args[5],
                ProjectDir = args[6],
                SubNamespace = args[7]
            };
        }
    }
}
