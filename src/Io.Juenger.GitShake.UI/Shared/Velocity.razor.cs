using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.DataAccess.GitLab.Configs;
using Scrummy.DataAccess.GitLab.Parsers;
using Scrummy.Scrum.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Models;
using Scrummy.Scrum.Providers;

namespace Scrummy.UI.Shared
{
    public partial class Velocity
    {
        private float _velocity;
        private float _bestVelocity;
        private float _worstVelocity;
        
        private IEnumerable<Xy<string, int>> _completedStoriesSeries;
        private IEnumerable<Xy<string, int>> _smaVelocitySeries;
        
        private readonly bool _smooth = false;
        
        [Inject]
        private IDataAccessConfig DataAccessConfig { get; set; }
        
        [Inject]
        private IChartProvider ChartProvider { get; set; }
        
        [Inject]
        private ISprintProvider SprintProvider { get; set; }
        
        [Inject]
        private IVelocityProvider VelocityProvider { get; set; }
        
        [Parameter]
        public IEnumerable<Story> Stories { get; set; }

        [Parameter] 
        public int SprintLength { get; set; } = 1;

        [Parameter]
        public DateTime StartDate { get; set; }
        
        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            
            if(Stories == null) return;

            var velocity = await VelocityProvider
                .GetVelocityAsync(DataAccessConfig.ProjectId)
                .ConfigureAwait(false);
            
            _velocity = velocity.AverageVelocity;
            _bestVelocity = velocity.Best3SprintsAverageVelocity;
            _worstVelocity = velocity.Worst3SprintsAverageVelocity;
            
            var sprints = await SprintProvider.GetAllSprintsAsync(DataAccessConfig.ProjectId);
            
            var series = ChartProvider.GetVelocityChartData(sprints);
            
            _completedStoriesSeries = series[0].Series;
            _smaVelocitySeries = series[^1].Series;
        }
    }
}