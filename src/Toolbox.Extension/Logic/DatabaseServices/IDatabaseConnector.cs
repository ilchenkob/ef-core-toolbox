﻿using System.Threading;
using System.Threading.Tasks;

namespace Toolbox.Extension.Logic.DatabaseServices
{
    public interface IDatabaseConnector
    {
        Task<bool> TryConnect(string connectionString, CancellationToken cancellationToken);
    }
}
