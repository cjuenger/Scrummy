using System;
using System.Collections.Generic;
using System.Linq;

namespace Scrummy.Scrum.Contracts.Models
{
    public class BurnDownChartData
    {
        public IEnumerable<Xy<DateTime, int>> BurnDownSeries { get; set; } = Enumerable.Empty<Xy<DateTime, int>>();
        public IEnumerable<Xy<DateTime, int>> EstimateSeries { get; set; } = Enumerable.Empty<Xy<DateTime, int>>();
        public IEnumerable<Xy<DateTime, int>> BestEstimateSeries { get; set; } = Enumerable.Empty<Xy<DateTime, int>>();
        public IEnumerable<Xy<DateTime, int>> WorstEstimateSeries { get; set; } = Enumerable.Empty<Xy<DateTime, int>>();
        
        public DateTime DueDate { get; set; }
    }
}