using System;
using System.Collections.Generic;
using Scrummy.DataAccess.Contracts.Models;

namespace Scrummy.Scrum.Providers
{
    public interface IChartGenerator
    {
        IEnumerable<Models.Xy<DateTime, int>> GetOpenedStoryChart(IEnumerable<Story> stories);
        
        IEnumerable<Models.Xy<DateTime, int>> GetCumulatedOpenedStoryChart(IEnumerable<Story> stories, bool tillToday = true);
        
        IEnumerable<Models.Xy<DateTime, int>> GetClosedStoryChart(IEnumerable<Story> stories);
        
        IEnumerable<Models.Xy<DateTime, int>> GetCumulatedClosedStoryChart(IEnumerable<Story> stories, bool tillToday = true);
        
        IEnumerable<Models.Xy<DateTime, int>> GetBurnUpChart(IEnumerable<Story> stories, bool tillToday = true);
        
        IEnumerable<Models.Xy<DateTime, int>> GetBurnDownChart(IEnumerable<Story> stories, bool tillToday = true);
        
        IEnumerable<Models.Xy<DateTime, int>> GetBurnDownEstimationChart(IEnumerable<Story> stories, double velocity);
        
    }
}