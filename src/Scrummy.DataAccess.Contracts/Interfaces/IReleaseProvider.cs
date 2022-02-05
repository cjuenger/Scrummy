using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scrummy.DataAccess.Contracts.Models;

namespace Scrummy.DataAccess.Contracts.Interfaces
{
    public interface IReleaseProvider
    {
        Task<Release> GetReleaseAsync(
            string projectId,
            ReleaseInfo releaseInfo, 
            CancellationToken ct = default);
    }
}