namespace Toolbox.Extension.Logic.Settings.Models
{
    public class MsSqlDatabaseConnectionSettings
    {
        public string Server { get; set; }

        public string Database { get; set; }

        public bool IsSqlAuthSelected { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
