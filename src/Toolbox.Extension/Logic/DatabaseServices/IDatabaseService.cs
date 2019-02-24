﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Toolbox.Extension.Logic.Scaffolding.DatabaseServices
{
    public interface IDatabaseService
    {
        Task<Dictionary<string, List<string>>> GetTables(string connectionString, CancellationToken cancellationToken);
    }
}
