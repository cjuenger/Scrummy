using System.Collections.Generic;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Metrics
{
    internal interface IBurnUpChartDataProvider
    {
        BurnUpChartData GetBurnUpChartData(IEnumerable<Item> items, bool tillToday = true);
    }
}