using System;
using Toolbox.Extension.Logic.Settings.Models;

namespace Toolbox.Extension.Logic.Settings
{
    public class SettingsStore
    {
        private static Lazy<SettingsStore> _instance = new Lazy<SettingsStore>(() => new SettingsStore
        {
            MsSqlDatabaseConnectionSettings = new MsSqlDatabaseConnectionSettings()
        });

        public static SettingsStore Instance => _instance.Value;

        public static void SetInstance(MsSqlDatabaseConnectionSettings msSqlConnectionSettings)
        {
            _instance = new Lazy<SettingsStore>(() => new SettingsStore
            {
                MsSqlDatabaseConnectionSettings = msSqlConnectionSettings
            });
        }

        private SettingsStore() { }

        public MsSqlDatabaseConnectionSettings MsSqlDatabaseConnectionSettings { get; set; }
    }
}
