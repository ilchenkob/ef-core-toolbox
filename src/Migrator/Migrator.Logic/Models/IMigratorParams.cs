namespace Migrator.Logic.Models
{
    public interface IMigratorParams
    {
        string Command { get; }

        string ToArgumentString();
    }
}
