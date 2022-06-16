using System.Collections.Generic;

namespace Scrummy.Scrum.Contracts.Models
{
    public class VelocityChartData
    {
        public Velocity Velocity { get; set; }

        public IReadOnlyList<DataSeries<string, int>> VelocitySeries { get; set; } = new List<DataSeries<string, int>>();
    }
}