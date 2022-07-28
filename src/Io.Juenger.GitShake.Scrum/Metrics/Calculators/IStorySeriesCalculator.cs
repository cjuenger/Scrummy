using System;
using System.Collections.Generic;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Metrics.Calculators
{
    internal interface IStorySeriesCalculator
    {
        IEnumerable<Xy<DateTime, int>> CalculateOpenedStoryChart(IEnumerable<Story> stories);
        
        IEnumerable<Xy<DateTime, int>> CalculateCumulatedOpenedStoryChart(IEnumerable<Story> stories, bool tillToday = true);
        
        IEnumerable<Xy<DateTime, int>> CalculateClosedStoryChart(IEnumerable<Story> stories);
        
        IEnumerable<Xy<DateTime, int>> CalculateCumulatedClosedStoryChart(IEnumerable<Story> stories, bool tillToday = true);
    }
}