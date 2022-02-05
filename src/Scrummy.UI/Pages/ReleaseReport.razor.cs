using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Scrummy.DataAccess.Contracts.Enums;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.DataAccess.Contracts.Models;
using Scrummy.UI.Configs;

namespace Scrummy.UI.Pages
{
    public partial class ReleaseReport
    {
        private ReleaseInfo _selectedReleaseInfo;
        
        private Release _selectedRelease;

        private IEnumerable<Item> OpenItems => 
            _selectedRelease?
                .Items
                .Where(i => i.State == WorkflowState.Opened);
        
        private IEnumerable<Item> ClosedItems => 
            _selectedRelease?
                .Items
                .Where(i => i.State == WorkflowState.Closed);

        private IEnumerable<Story> Stories =>
            _selectedRelease?
                .Items
                .OfType<Story>();
        
        [Inject]
        private IReleaseInfoProvider ReleaseInfoProvider { get; set; }

        [Inject]
        private IReleaseProvider ReleaseProvider { get; set; }
        
        [Inject]
        private IGitLabConfig GitLabConfig { get; set; }
        
        [Inject]
        private IScrumConfig ScrumConfig { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);

            var releaseInfos = 
                await ReleaseInfoProvider
                    .GetAllReleasesAsync(GitLabConfig.ProjectId)
                    .ConfigureAwait(false);

            var (isSuccess, releaseInfo) = await ReleaseInfoProvider
                .TryGetNextUpcomingReleaseAsync(GitLabConfig.ProjectId)
                .ConfigureAwait(false);

            if (isSuccess)
            {
                _selectedReleaseInfo = releaseInfo;
                _selectedRelease = 
                    await ReleaseProvider
                        .GetReleaseAsync(GitLabConfig.ProjectId, releaseInfo)
                        .ConfigureAwait(false);
            }
            else if (releaseInfos != null)
            {
                _selectedReleaseInfo = releaseInfos[^1];
                _selectedRelease = 
                    await ReleaseProvider
                        .GetReleaseAsync(GitLabConfig.ProjectId, _selectedReleaseInfo)
                        .ConfigureAwait(false);
            }
        }
    }
}