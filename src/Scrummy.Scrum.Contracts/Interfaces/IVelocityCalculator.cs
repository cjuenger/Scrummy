using System.Collections.Generic;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Contracts.Interfaces
{
    public interface IVelocityCalculator
    {
        Velocity CalculateVelocity(IEnumerable<Sprint> sprints);
    }
}