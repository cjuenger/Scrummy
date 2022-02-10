﻿using System;
using System.Collections.Generic;
using Scrummy.DataAccess.Contracts.Models;
using Scrummy.Scrum.Models;

namespace Scrummy.Scrum.Providers
{
    public interface IChartGenerator
    {
        IEnumerable<Xy<DateTime, int>> GetOpenedStoryChart(IEnumerable<Story> stories);
        
        IEnumerable<Xy<DateTime, int>> GetCumulatedOpenedStoryChart(IEnumerable<Story> stories, bool tillToday = true);
        
        IEnumerable<Xy<DateTime, int>> GetClosedStoryChart(IEnumerable<Story> stories);
        
        IEnumerable<Xy<DateTime, int>> GetCumulatedClosedStoryChart(IEnumerable<Story> stories, bool tillToday = true);
        
        IEnumerable<Xy<DateTime, int>> GetBurnUpChart(IEnumerable<Story> stories, bool tillToday = true);
        
        IEnumerable<Xy<DateTime, int>> GetBurnDownChart(IEnumerable<Story> stories, bool tillToday = true);
        
        IEnumerable<Xy<DateTime, int>> GetBurnDownEstimationChart(IEnumerable<Story> stories, double velocityPerDay);

        IEnumerable<Xy<DateTime, int>> GetVelocityChart(IEnumerable<Sprint> sprints, bool tillToday = true);
    }
}