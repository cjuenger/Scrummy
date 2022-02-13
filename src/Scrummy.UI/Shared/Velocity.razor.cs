using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IO.Juenger.GitLab.Api;
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
        private int _velocity;
        
        private IEnumerable<Xy<DateTime, int>> _velocitySeries;
        
        private readonly bool _smooth = false;
        
        [Inject]
        private IGitLabConfig GitLabConfig { get; set; }
        
        [Inject]
        private IChartService ChartService { get; set; }
        
        [Inject]
        private ISprintProvider SprintProvider { get; set; }
        
        [Inject]
        private IVelocityProvider VelocityCalculator { get; set; }
        
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

            await VelocityCalculator.CalculateVelocityAsync(GitLabConfig.ProjectId).ConfigureAwait(false);

            var sprints = await SprintProvider.GetAllSprintsAsync("28355012");
            _velocitySeries = ChartService.GetVelocityChart(sprints);
        }
    }
}