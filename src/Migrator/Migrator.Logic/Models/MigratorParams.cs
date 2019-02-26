namespace Migrator.Logic.Models
{
    public abstract class MigratorParams
    {
        public abstract string Command { get; }

        public abstract string ToArgumentString();
    }
}
