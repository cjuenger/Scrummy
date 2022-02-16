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
        
        private IEnumerable<Xy<DateTime, int>> _velocitySeries;
        private readonly bool _smooth = false;
        
        [Inject]
        private IDataAccessConfig DataAccessConfig { get; set; }
        
        [Inject]
        private IChartService ChartService { get; set; }
        
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

            await VelocityProvider.CalculateVelocityAsync(DataAccessConfig.ProjectId).ConfigureAwait(false);
            _velocity = VelocityProvider.Velocity.AverageVelocity;
            _bestVelocity = VelocityProvider.Velocity.Best3SprintsAverageVelocity;
            _worstVelocity = VelocityProvider.Velocity.Worst3SprintsAverageVelocity;
            
            var sprints = await SprintProvider.GetAllSprintsAsync(DataAccessConfig.ProjectId);
            _velocitySeries = ChartService.GetVelocityChart(sprints);
            
        }
    }
}