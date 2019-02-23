using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Extension.Logic.Settings.Models;

namespace Toolbox.Extension.Logic.Settings
{
    public class Settings
    {
        private static Lazy<Settings> _instance = new Lazy<Settings>(() => new Settings());

        public static Settings Instance => _instance.Value;

        public static void SetInstance(Settings instance)
        {
            _instance = new Lazy<Settings>(() => instance);
        }

        private Settings() { }

        public MsSqlDatabaseConnectionSettings MsSqlDatabaseConnectionSettings { get; set; }
    }
}
