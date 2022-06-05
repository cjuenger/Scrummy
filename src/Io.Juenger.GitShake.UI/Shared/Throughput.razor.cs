using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.UI.Shared
{
    public partial class Throughput
    {
        [Inject]
        private IDataAccessConfig DataAccessConfig { get; set; }
        
        [Inject]
        private IThroughputProvider ThroughputProvider { get; set; }
        
        private TimeSpan AverageStoryThroughputTime { get; set; }
        private TimeSpan BestStoryThroughputTime { get; set; }
        private TimeSpan WorstStoryThroughputTime { get; set; }
        
        private TimeSpan AverageBugThroughputTime { get; set; }
        private TimeSpan BestBugThroughputTime { get; set; }
        private TimeSpan WorstBugThroughputTime { get; set; }
        
        private TimeSpan AverageOtherThroughputTime { get; set; }
        private TimeSpan BestOtherThroughputTime { get; set; }
        private TimeSpan WorstOtherThroughputTime { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);

            var ThroughputTime = await ThroughputProvider
                .GetThroughputTimeAsync(DataAccessConfig.ProjectId)
                .ConfigureAwait(false);

            AverageStoryThroughputTime = ThroughputTime.AverageStoryThroughputTime;
            BestStoryThroughputTime = ThroughputTime.BestStoryThroughputTime;
            WorstStoryThroughputTime = ThroughputTime.WorstStoryThroughputTime;

            AverageBugThroughputTime = ThroughputTime.AverageBugThroughputTime;
            BestBugThroughputTime = ThroughputTime.BestBugThroughputTime;
            WorstBugThroughputTime = ThroughputTime.WorstBugThroughputTime;

            AverageOtherThroughputTime = ThroughputTime.AverageOtherThroughputTime;
            BestOtherThroughputTime = ThroughputTime.BestOtherThroughputTime;
            WorstOtherThroughputTime = ThroughputTime.WorstOtherThroughputTime;
        }
    }
}