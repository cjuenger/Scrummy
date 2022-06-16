using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Contracts.Interfaces
{
    public interface IChartProvider
    {
        IEnumerable<Xy<DateTime, int>> GetOpenedStoryChart(IEnumerable<Story> stories);
        
        IEnumerable<Xy<DateTime, int>> GetCumulatedOpenedStoryChart(IEnumerable<Story> stories, bool tillToday = true);
        
        IEnumerable<Xy<DateTime, int>> GetClosedStoryChart(IEnumerable<Story> stories);
        
        IEnumerable<Xy<DateTime, int>> GetCumulatedClosedStoryChart(IEnumerable<Story> stories, bool tillToday = true);
        
        Task<BurnDownChartData> GetProjectBurnDownChartDataAsync(string projectId, CancellationToken ct = default);
        
        Task<BurnDownChartData> GetSprintBurnDownChartDataAsync(string projectId, int sprintId, CancellationToken ct = default);

        Task<BurnDownChartData> GetReleaseBurnDownChartDataAsync(string projectId, int releaseId, CancellationToken ct = default);
        
        Task<BurnUpChartData> GetProjectBurnUpChartDataAsync(string projectId, CancellationToken ct = default);
        
        Task<BurnUpChartData> GetSprintBurnUpChartDataAsync(string projectId, int sprintId, CancellationToken ct = default);
        
        Task<BurnUpChartData> GetReleaseBurnUpChartDataAsync(string projectId, int releaseId, CancellationToken ct = default);
        
        Task<VelocityChartData> GetVelocityChartData(string projectId, CancellationToken ct = default);
    }
}