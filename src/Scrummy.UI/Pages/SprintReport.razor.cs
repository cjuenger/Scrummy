using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Scrummy.DataAccess.Contracts.Enums;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Models;
using Scrummy.UI.Configs;

namespace Scrummy.UI.Pages
{
    public partial class SprintReport
    {
        private IEnumerable<SprintInfo> _sprintInfos;
        private SprintInfo _selectedSprintInfo;
        private Sprint _selectedSprint;

        private DateTime? _startTime;
        private DateTime? _endTime;
        
        private IEnumerable<Item> OpenItems => 
            _selectedSprint?
                .Items
                .Where(i => i.State == WorkflowState.Opened) ?? Enumerable.Empty<Item>();
        
        private IEnumerable<Item> ClosedItems => 
            _selectedSprint?
                .Items
                .Where(i => i.State == WorkflowState.Closed) ?? Enumerable.Empty<Item>();

        private IEnumerable<Item> TotalItems => _selectedSprint?.Items;
        
        private IEnumerable<Story> Stories =>
            _selectedSprint?
                .Items
                .OfType<Story>() ?? Enumerable.Empty<Story>();
        
        [Inject]
        private ISprintInfoProvider SprintInfoProvider { get; set; }

        [Inject]
        private ISprintProvider SprintProvider { get; set; }
        
        [Inject]
        private IDataAccessConfig DataAccessConfig { get; set; }
        
        [Inject]
        private IScrumConfig ScrumConfig { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);

            _sprintInfos = 
                await SprintInfoProvider
                    .GetSprintInfosAsync(DataAccessConfig.ProjectId)
                    .ConfigureAwait(false);

            var (isSuccess, sprintInfo) = await SprintInfoProvider
                .TryGetCurrentSprintInfoAsync(DataAccessConfig.ProjectId)
                .ConfigureAwait(false);

            if (isSuccess)
            {
                _selectedSprintInfo = sprintInfo;
            }
            else if (_sprintInfos != null)
            {
                _selectedSprintInfo = _sprintInfos.Last();
            }
            
            await LoadSprintAsync(_selectedSprintInfo).ConfigureAwait(false);
        }

        private async Task LoadSprintAsync(SprintInfo sprintInfo)
        {
            _selectedSprint = 
                await SprintProvider
                    .GetSprintAsync(DataAccessConfig.ProjectId, sprintInfo)
                    .ConfigureAwait(false);

            _endTime = _selectedSprint.Info.EndTime;
        }
        
        private async void OnChange(object value)
        {
            switch (value)
            {
                case null:
                    _selectedSprintInfo = null;
                    _selectedSprint = null;
                    break;
                case SprintInfo sprintInfo:
                    await LoadSprintAsync(sprintInfo).ConfigureAwait(false);
                    break;
            }
            
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);

            Debug.WriteLine($"Value of {value} changed");
        }
    }
}