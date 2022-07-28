using System.Threading;
using System.Threading.Tasks;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Contracts.Interfaces
{
    public interface IBurnDownChartDataProvider
    {
        Task<BurnDownChartData> GetProjectBurnDownChartDataAsync(string projectId, CancellationToken ct = default);
        
        Task<BurnDownChartData> GetReleaseBurnDownChartDataAsync(string projectId, int releaseId, CancellationToken ct = default);
    }
}