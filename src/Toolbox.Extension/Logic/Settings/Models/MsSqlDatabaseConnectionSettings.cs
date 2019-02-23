using System;

namespace Toolbox.Extension.Logic.Settings.Models
{
    [Serializable]
    public class MsSqlDatabaseConnectionSettings
    {
        public MsSqlDatabaseConnectionSettings()
        {
            Server = Database = Username = Password = string.Empty;
            IsSqlAuthSelected = false;
        }

        public string Server { get; set; }

        public string Database { get; set; }

        public bool IsSqlAuthSelected { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
