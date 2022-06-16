using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Metrics
{
    internal interface IBurnDownChartDataProvider
    {
        IEnumerable<Xy<DateTime, int>> GetBurnDownChartSeries(IEnumerable<Story> stories, bool tillToday = true);
        IEnumerable<Xy<DateTime, int>> GetBurnDownEstimationChartSeries(IEnumerable<Story> stories, float velocityPerDay);

        Task<BurnDownChartData> GetBurnDownChartDataAsync(
            string projectId, 
            IEnumerable<Item> items, 
            CancellationToken ct);
    }
}