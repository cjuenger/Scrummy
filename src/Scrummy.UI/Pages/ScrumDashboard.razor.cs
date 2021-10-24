using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IO.Juenger.GitLab.Api;
using IO.Juenger.GitLab.Model;
using Microsoft.AspNetCore.Components;
using Scrummy.GitLab.Contracts.Parsers;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.UI.Pages
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public partial class ScrumDashboard
    {
        [Inject]
        private IProjectApi ProjectApi { get; set; }
        
        [Inject]
        private IItemParser ItemParser { get; set; }

        private string _projectId = "28355012";

        private int _sprintLength = 2;

        private DateTime _startDate;

        private List<Issue> _issues;

        private List<Item> _items;

        private List<Story> _stories;

        public ScrumDashboard()
        {
            _startDate = new DateTime(2021, 08, 01);
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _issues = await LoadIssuesAsync();
            _items = GetItemsFromIssues(_issues);
            _stories = GetStoriesFromItems(_items);
        }

        private async Task<List<Issue>> LoadIssuesAsync()
        {
            var issues = await ProjectApi
                .GetProjectIssuesAsync(_projectId)
                .ConfigureAwait(false);

            return issues;
        }

        private List<Item> GetItemsFromIssues(IEnumerable<Issue> issues)
        {
            var items = issues.Select(ItemParser.Parse);
            return items.ToList();
        }

        private static List<Story> GetStoriesFromItems(IEnumerable<Item> items)
        {
            return items.OfType<Story>().ToList();
        }
    }
}