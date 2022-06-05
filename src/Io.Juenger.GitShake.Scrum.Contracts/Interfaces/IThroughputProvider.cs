﻿using System.Threading;
using System.Threading.Tasks;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Contracts.Interfaces
{
    public interface IThroughputProvider
    {
        Task<Throughput> GetThroughputTimeAsync(string projectId, CancellationToken ct = default);
    }
}