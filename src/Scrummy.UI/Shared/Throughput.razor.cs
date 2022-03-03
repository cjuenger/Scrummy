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
        private IPassThroughTimeProvider PassThroughTimeProvider { get; set; }
        
        public PassThroughTime PassThroughTime { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);

            PassThroughTime = await PassThroughTimeProvider
                .GetPassThroughTimeAsync(DataAccessConfig.ProjectId)
                .ConfigureAwait(false);
        }
    }
}