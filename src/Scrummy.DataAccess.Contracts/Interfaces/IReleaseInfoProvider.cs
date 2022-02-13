using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.DataAccess.Contracts.Interfaces
{
    public interface IReleaseInfoProvider
    {
        Task<IReadOnlyList<ReleaseInfo>> GetAllReleasesAsync(string projectId, CancellationToken ct = default);
        Task<(bool IsSuccess, ReleaseInfo ReleaseInfo)> TryGetNextUpcomingReleaseAsync(string projectId, CancellationToken ct = default);
    }
}