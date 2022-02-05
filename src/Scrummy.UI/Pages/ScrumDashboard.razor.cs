using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IO.Juenger.GitLab.Api;
using IO.Juenger.GitLab.Model;
using Microsoft.AspNetCore.Components;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.DataAccess.Contracts.Models;
using Scrummy.DataAccess.GitLab.Configs;
using Scrummy.UI.Configs;

namespace Scrummy.UI.Pages
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public partial class ScrumDashboard
    {
        [Inject] 
        private ISprintProvider SprintProvider { get; set; }
        
        [Inject]
        private IProjectApi ProjectApi { get; set; }
        
        [Inject]
        private IItemsProvider ItemsProvider { get; set; }

        [Inject]
        private IGitLabConfig GitLabConfig { get; set; }
        
        [Inject]
        private IScrumConfig ScrumConfig { get; set; }
        
        private List<Issue> _issues;

        private IEnumerable<Item> _items;

        private List<Story> _stories;

        private IEnumerable<Sprint> _sprints;

        private Sprint _currentSprint;
        
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);
            _issues = await GetIssuesAsync().ConfigureAwait(false);
            _items = await GetItemsAsync().ConfigureAwait(false);
            _stories = GetStoriesFromItems(_items);
            _sprints = await GetAllSprints().ConfigureAwait(false);
            _currentSprint = await GetCurrentSprint().ConfigureAwait(false);
        }

        private async Task<IEnumerable<Sprint>> GetAllSprints()
        {
            var sprints = await SprintProvider.GetAllSprintsAsync(GitLabConfig.ProjectId);

            foreach (var sprint in sprints)
            {
                Debug.WriteLine(
                    $"{sprint.Name} Start:{sprint.StartTime} End:{sprint.EndTime} Items: {sprint.Items.Count}");
            }

            return sprints;
        }

        private async Task<Sprint> GetCurrentSprint()
        {
            var (isSuccess, sprint) = await SprintProvider.TryGetCurrentSprintAsync(GitLabConfig.ProjectId);
            Debug.WriteLine($"The current sprint is {isSuccess}");

            return sprint;
        }
        
        private async Task<List<Issue>> GetIssuesAsync()
        {
            var sprints = await SprintProvider.GetAllSprintsAsync(GitLabConfig.ProjectId);

            foreach (var sprint in sprints)
            {
                Debug.WriteLine(
                    $"{sprint.Name} Start:{sprint.StartTime} End:{sprint.EndTime} Items: {sprint.Items.Count}");
            }

            var request = await SprintProvider.TryGetCurrentSprintAsync(GitLabConfig.ProjectId);
            Debug.WriteLine($"The current sprint is {request.IsSuccess}");
            
            var issues = await ProjectApi
                .GetProjectIssuesAsync(GitLabConfig.ProjectId)
                .ConfigureAwait(false);

            return issues;
        }

        private async Task<IEnumerable<Item>> GetItemsAsync()
        {
            var items = await ItemsProvider.GetAllItemsAsync(GitLabConfig.ProjectId);
            return items.ToList();
        }

        private static List<Story> GetStoriesFromItems(IEnumerable<Item> items)
        {
            return items.OfType<Story>().ToList();
        }
    }
}