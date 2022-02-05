using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Scrummy.DataAccess.Contracts.Models;
using Scrummy.Scrum.Metrics;
using Scrummy.Scrum.Models;
using Scrummy.Scrum.Providers;

namespace Scrummy.UI.Shared
{
    public partial class ReleaseBurnDownChart
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
        
        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync().ConfigureAwait(false);
            
            if(Stories == null || !Stories.Any())
            {
                _burnDown = Enumerable.Empty<Xy<DateTime, int>>();
                _estimate = Enumerable.Empty<Xy<DateTime, int>>();
                
                await InvokeAsync(StateHasChanged).ConfigureAwait(false);
                
                return;
            }
            
            _burnDown = ChartGenerator.GetBurnDownChart(Stories);
            
            var velocity = VelocityCalculator.GetAverageVelocityPerSingleDay(Stories, StartDate, DateTime.Now);
            _estimate = ChartGenerator.GetBurnDownEstimationChart(Stories, velocity);
        }
    }
}