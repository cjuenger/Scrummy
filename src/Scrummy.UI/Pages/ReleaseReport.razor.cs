using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Scrummy.DataAccess.Contracts.Enums;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.DataAccess.GitLab.Configs;
using Scrummy.Scrum.Contracts.Models;
using Scrummy.UI.Configs;

namespace Scrummy.UI.Pages
{
    public partial class ReleaseReport
    {
        private IEnumerable<ReleaseInfo> _releaseInfos;
        private ReleaseInfo _selectedReleaseInfo;
        private Release _selectedRelease;

        private DateTime _startDate;
        private DateTime? _dueDate;
        
        private IEnumerable<Item> OpenItems => 
            _selectedRelease?
                .Items
                .Where(i => i.State == WorkflowState.Opened) ?? Enumerable.Empty<Item>();
        
        private IEnumerable<Item> ClosedItems => 
            _selectedRelease?
                .Items
                .Where(i => i.State == WorkflowState.Closed) ?? Enumerable.Empty<Item>();

        private IEnumerable<Item> TotalItems => _selectedRelease?.Items;
        
        private IEnumerable<Story> Stories =>
            _selectedRelease?
                .Items
                .OfType<Story>() ?? Enumerable.Empty<Story>();
        
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

            _startDate = ScrumConfig.ProjectStart;
            
            _releaseInfos = 
                await ReleaseInfoProvider
                    .GetAllReleasesAsync(GitLabConfig.ProjectId)
                    .ConfigureAwait(false);

            var (isSuccess, releaseInfo) = await ReleaseInfoProvider
                .TryGetNextUpcomingReleaseAsync(GitLabConfig.ProjectId)
                .ConfigureAwait(false);

            if (isSuccess)
            {
                _selectedReleaseInfo = releaseInfo;
            }
            else if (_releaseInfos != null)
            {
                _selectedReleaseInfo = _releaseInfos.Last();
            }
            
            await LoadReleaseAsync(_selectedReleaseInfo).ConfigureAwait(false);
        }

        private async Task LoadReleaseAsync(ReleaseInfo releaseInfo)
        {
            _selectedRelease = 
                await ReleaseProvider
                    .GetReleaseAsync(GitLabConfig.ProjectId, releaseInfo)
                    .ConfigureAwait(false);

            _dueDate = _selectedRelease.Info.DueDate;
        }
        
        private async void OnChange(object value)
        {
            switch (value)
            {
                case null:
                    _selectedReleaseInfo = null;
                    _selectedRelease = null;
                    break;
                case ReleaseInfo releaseInfo:
                    await LoadReleaseAsync(releaseInfo).ConfigureAwait(false);
                    break;
            }
            
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);

            Debug.WriteLine($"Value of {value} changed");
        }
    }
}