using System.Collections.Generic;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Metrics.Calculators
{
    internal interface IVelocityCalculator
    {
        Velocity CalculateVelocity(IEnumerable<Sprint> sprints);
        
        VelocityChartData CalculateVelocityChartData(IEnumerable<Sprint> sprints);
    }
}