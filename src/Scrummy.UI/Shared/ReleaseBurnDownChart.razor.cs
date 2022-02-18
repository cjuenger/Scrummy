using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.DataAccess.GitLab.Configs;
using Scrummy.Scrum.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Models;
using Scrummy.Scrum.Providers;

namespace Scrummy.UI.Shared
{
    public partial class ReleaseBurnDownChart
    {
        private int _maxYValue;
        
        private IEnumerable<Xy<DateTime, int>> _burnDown;
        private IEnumerable<Xy<DateTime, int>> _estimate;
        private IEnumerable<Xy<DateTime, int>> _bestEstimate;
        private IEnumerable<Xy<DateTime, int>> _worstEstimate;
        private IEnumerable<Xy<DateTime, int>> _dueLine;
        
        private readonly bool _smooth = false;
        
        [Inject]
        private IDataAccessConfig DataAccessConfig { get; set; }
        
        [Inject]
        private IVelocityProvider VelocityProvider { get; set; }
        
        [Inject]
        private IChartService ChartService { get; set; }
        
        [Parameter] 
        public IEnumerable<Story> Stories { get; set; }

        [Parameter]
        public DateTime StartDate { get; set; }
        
        [Parameter]
        public DateTime? DueDate { get; set; }

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
            
            _burnDown = ChartService.GetBurnDownChart(Stories);

            _maxYValue = _burnDown.Select(bd => bd.Y).Max() + 5;
            
            var velocity = await VelocityProvider.GetVelocityAsync(DataAccessConfig.ProjectId).ConfigureAwait(false);
            
            _estimate = ChartService
                .GetBurnDownEstimationChart(Stories, velocity.DayAverageVelocity);
            
            _bestEstimate = ChartService
                .GetBurnDownEstimationChart(Stories, velocity.Best3SprintsDayAverageVelocity);
            
            _worstEstimate = ChartService
                .GetBurnDownEstimationChart(Stories, velocity.Worst3SprintsDayAverageVelocity);

            CalculateDueLine();
        }

        private void CalculateDueLine()
        {
            if (DueDate.HasValue)
            {
                _dueLine = new Xy<DateTime, int>[]
                {
                    new()
                    {
                        X = DueDate.Value, 
                        Y = 0
                    },
                    new()
                    {
                        X = DueDate.Value, 
                        Y = _maxYValue
                    },
                };
            }
        }
    }
}