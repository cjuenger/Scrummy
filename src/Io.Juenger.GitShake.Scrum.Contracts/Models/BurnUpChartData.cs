using System;
using System.Collections.Generic;
using System.Linq;

namespace Scrummy.Scrum.Contracts.Models
{
    public class BurnUpChartData
    {
        public IEnumerable<Xy<DateTime, int>> BurnUpSeries { get; set; } = Enumerable.Empty<Xy<DateTime, int>>();
    }
}