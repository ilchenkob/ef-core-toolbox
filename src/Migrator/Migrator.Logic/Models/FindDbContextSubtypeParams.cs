namespace Migrator.Logic.Models
{
    public class FindDbContextSubtypeParams : IMigratorParams
    {
        public string Command => CommandType.FindDbContextSubtypes;

        public string AssemblyFileName { get; set; }

        public string ToArgumentString()
        {
            return $"{Command} \"{AssemblyFileName}\"";
        }

        public static FindDbContextSubtypeParams FromArgumentsArray(string[] args)
        {
            if (args == null || args.Length < 2)
                throw new System.ArgumentException($"Invalid arguments. Can not construct {nameof(FindDbContextSubtypeParams)}");

            return new FindDbContextSubtypeParams
            {
                AssemblyFileName = args[1]
            };
        }
    }
}
