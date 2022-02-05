﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scrummy.DataAccess.Contracts.Models;

namespace Scrummy.DataAccess.Contracts.Interfaces
{
    public interface IItemsProvider
    {
        Task<IEnumerable<Item>> GetAllItemsAsync(string projectId, CancellationToken ct = default);
    }
}