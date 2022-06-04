using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.DataAccess.Contracts.Interfaces
{
    public interface ISprintInfoProvider
    {
        Task<IReadOnlyList<SprintInfo>> GetSprintInfosAsync(string projectId, CancellationToken ct = default);
        Task<(bool IsSuccess, SprintInfo SprintInfo)> TryGetCurrentSprintInfoAsync(
            string projectId, 
            CancellationToken ct = default);
        Task<(bool IsSuccess, SprintInfo SprintInfo)> TryGetSprintInfoAsync(
            string projectId, 
            int sprintId, 
            CancellationToken ct = default);
    }
}