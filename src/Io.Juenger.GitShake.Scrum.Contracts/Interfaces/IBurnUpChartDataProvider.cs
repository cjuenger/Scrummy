using System.Threading;
using System.Threading.Tasks;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Contracts.Interfaces
{
    public interface IBurnUpChartDataProvider
    {
        Task<BurnUpChartData> GetProjectBurnUpChartDataAsync(string projectId, CancellationToken ct = default);
        
        Task<BurnUpChartData> GetReleaseBurnUpChartDataAsync(string projectId, int releaseId, CancellationToken ct = default);
    }
}