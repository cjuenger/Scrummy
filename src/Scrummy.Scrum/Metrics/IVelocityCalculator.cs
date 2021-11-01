using System;
using System.Collections.Generic;
using Scrummy.DataAccess.Contracts.Models;

namespace Scrummy.Scrum.Metrics
{
    public interface IVelocityCalculator
    {
        int GetAverageVelocityPerSprintWeeks(IEnumerable<Story> stories, DateTime start, DateTime end, int sprintWeeks);
        
        double GetAverageVelocityPerSingleDay(IEnumerable<Story> stories, DateTime start, DateTime end);

        double GetAverageVelocityPerDaySinceStart(IEnumerable<Story> stories, DateTime start);
    }
}