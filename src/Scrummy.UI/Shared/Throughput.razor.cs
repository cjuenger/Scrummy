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
        private IPassThroughProvider PassThroughProvider { get; set; }
        
        private TimeSpan AverageStoryPassThroughTime { get; set; }
        private TimeSpan BestStoryPassThroughTime { get; set; }
        private TimeSpan WorstStoryPassThroughTime { get; set; }
        
        private TimeSpan AverageBugPassThroughTime { get; set; }
        private TimeSpan BestBugPassThroughTime { get; set; }
        private TimeSpan WorstBugPassThroughTime { get; set; }
        
        private TimeSpan AverageOtherPassThroughTime { get; set; }
        private TimeSpan BestOtherPassThroughTime { get; set; }
        private TimeSpan WorstOtherPassThroughTime { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);

            var passThroughTime = await PassThroughProvider
                .GetPassThroughTimeAsync(DataAccessConfig.ProjectId)
                .ConfigureAwait(false);

            AverageStoryPassThroughTime = passThroughTime.AverageStoryPassThroughTime;
            BestStoryPassThroughTime = passThroughTime.BestStoryPassThroughTime;
            WorstStoryPassThroughTime = passThroughTime.WorstStoryPassThroughTime;

            AverageBugPassThroughTime = passThroughTime.AverageBugPassThroughTime;
            BestBugPassThroughTime = passThroughTime.BestBugPassThroughTime;
            WorstBugPassThroughTime = passThroughTime.WorstBugPassThroughTime;

            AverageOtherPassThroughTime = passThroughTime.AverageOtherPassThroughTime;
            BestOtherPassThroughTime = passThroughTime.BestOtherPassThroughTime;
            WorstOtherPassThroughTime = passThroughTime.WorstOtherPassThroughTime;
        }
    }
}