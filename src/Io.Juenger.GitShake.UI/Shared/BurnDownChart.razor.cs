using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.DataAccess.GitLab.Configs;
using Scrummy.Scrum.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Models;
using Scrummy.Scrum.Metrics;

namespace Scrummy.UI.Shared
{
    public partial class BurnDownChart
    {
        private int _maxYValue;

        private BurnDownChartData _burnDownChartData;
        private IEnumerable<Xy<DateTime, int>> _dueLine;
        
        private readonly bool _smooth = false;
        
        [Inject]
        private IDataAccessConfig DataAccessConfig { get; set; }
        
        [Inject]
        private IChartProvider ChartProvider { get; set; }
        
        [Parameter] 
        public IEnumerable<Story> Stories { get; set; }

        [Parameter]
        public string Title { get; set; }
        
        [Parameter]
        public DateTime? DueDate { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync().ConfigureAwait(false);
            
            if(Stories == null || !Stories.Any())
            {
                _burnDownChartData = new BurnDownChartData();
                await InvokeAsync(StateHasChanged).ConfigureAwait(false);
                return;
            }

            _maxYValue = _burnDownChartData.BurnDownSeries.Select(bd => bd.Y).Max() + 5;

            _burnDownChartData = await ChartProvider.GetSprintBurnDownChartDataAsync(DataAccessConfig.ProjectId);

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