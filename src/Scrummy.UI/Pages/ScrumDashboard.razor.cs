using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IO.Juenger.GitLab.Api;
using IO.Juenger.GitLab.Model;
using Microsoft.AspNetCore.Components;
using Scrummy.DataAccess.Contracts.Models;
using Scrummy.DataAccess.Contracts.Providers;
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
        
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _issues = await LoadIssuesAsync();
            _items = await GetItemsAsync();
            _stories = GetStoriesFromItems(_items);
        }

        private async Task<List<Issue>> LoadIssuesAsync()
        {
            var sprints = await SprintProvider.GetAllSprintsAsync(GitLabConfig.ProjectId);

            foreach (var sprint in sprints)
            {
                Debug.WriteLine(
                    $"{sprint.Name} Start:{sprint.StartTime} End:{sprint.EndTime} Items: {sprint.Items.Count}");
            }

            var currentSprint = await SprintProvider.GetCurrentSprintAsync(GitLabConfig.ProjectId);
            Debug.WriteLine($"The current sprint is {currentSprint.Name}");
            
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