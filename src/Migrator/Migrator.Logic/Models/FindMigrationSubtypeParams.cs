namespace Migrator.Logic.Models
{
    public class FindMigrationSubtypeParams : IMigratorParams
    {
        public string Command => CommandType.FindMigrationSubtypes;

        public string AssemblyFileName { get; set; }

        public string ToArgumentString()
        {
            return $"{Command} \"{AssemblyFileName}\"";
        }

        public static FindMigrationSubtypeParams FromArgumentsArray(string[] args)
        {
            if (args == null || args.Length < 2)
                throw new System.ArgumentException($"Invalid arguments. Can not construct {nameof(FindMigrationSubtypeParams)}");

            return new FindMigrationSubtypeParams
            {
                AssemblyFileName = args[1]
            };
        }
    }
}
