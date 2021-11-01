using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Scrummy.DataAccess.Contracts.Models;
using Scrummy.Scrum.Metrics;
using Scrummy.Scrum.Models;
using Scrummy.Scrum.Providers;

namespace Scrummy.UI.Shared
{
    public partial class BurnDownChart
    {
        private IEnumerable<Xy<DateTime, int>> _burnDown;

        private IEnumerable<Xy<DateTime, int>> _estimate;
        
        private readonly bool _smooth = false;
        
        [Inject]
        private IVelocityCalculator VelocityCalculator { get; set; }
        
        [Inject]
        private IChartGenerator ChartGenerator { get; set; }
        
        [Parameter] 
        public IEnumerable<Story> Stories { get; set; }

        [Parameter]
        public DateTime StartDate { get; set; }
        
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            
            if(Stories == null || !Stories.Any()) return;
            
            _burnDown = ChartGenerator.GetBurnDownChart(Stories);
            
            var velocity = VelocityCalculator.GetAverageVelocityPerSingleDay(Stories, StartDate, DateTime.Now);
            _estimate = ChartGenerator.GetBurnDownEstimationChart(Stories, velocity);
        }
    }
}