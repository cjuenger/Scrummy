using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scrummy.DataAccess.Contracts.Models;

namespace Scrummy.DataAccess.Contracts.Providers
{
    public interface ISprintProvider
    {
        Task<(bool IsSuccess, Sprint Sprint)> TryGetCurrentSprintAsync(string projectId, CancellationToken ct = default);

        Task<IEnumerable<Sprint>> GetAllSprintsAsync(string projectId, CancellationToken ct = default);

        Task<IEnumerable<Sprint>> GetSprintsInRange(
            string projectId, 
            DateTime startTime, 
            DateTime endTime, 
            CancellationToken ct = default);
    }
}