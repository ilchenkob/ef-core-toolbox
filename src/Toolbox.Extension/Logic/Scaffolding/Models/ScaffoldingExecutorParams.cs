using System.Collections.Generic;
using Toolbox.Extension.Logic.Models;

namespace Toolbox.Extension.Logic.Scaffolding.Models
{
    public class ScaffoldingExecutorParams
    {
        public string ConnectionString { get; set; }

        public List<string> Tables { get; set; }

        public string DbContextNamespace { get; set; }

        public string DbContextClassName { get; set; }

        public string OutputFolder { get; set; }

        public bool UseDatabaseNames { get; set; }

        public bool UseDataAnnotations { get; set; }
    }
}
