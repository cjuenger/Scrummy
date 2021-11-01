using System;
using System.Collections.Generic;
using Scrummy.DataAccess.Contracts.Models;

namespace Scrummy.Scrum.Metrics
{
    public interface IVelocityCalculator
    {
        int GetVelocityPerSprint(IEnumerable<Story> stories, DateTime start, DateTime end, int sprintWeeks);
        double GetVelocityPerDay(IEnumerable<Story> stories, DateTime start, DateTime end);
    }
}