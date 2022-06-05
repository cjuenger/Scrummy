using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Models;
using Scrummy.UI.Configs;

namespace Scrummy.UI.Pages
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public partial class ScrumDashboard
    {
        [Inject] 
        private ISprintProvider SprintProvider { get; set; }
        
        [Inject]
        private IItemsProvider ItemsProvider { get; set; }

        [Inject]
        private IDataAccessConfig DataAccessConfig { get; set; }
        
        [Inject]
        private IScrumConfig ScrumConfig { get; set; }

        private IEnumerable<Item> _items;

        private List<Story> _stories;
        
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);
            _items = await GetItemsAsync().ConfigureAwait(false);
            _stories = GetStoriesFromItems(_items);
        }

        private async Task<IEnumerable<Item>> GetItemsAsync()
        {
            var items = await ItemsProvider.GetItemsOfProjectAsync(DataAccessConfig.ProjectId);
            return items.ToList();
        }

        private static List<Story> GetStoriesFromItems(IEnumerable<Item> items)
        {
            return items.OfType<Story>().ToList();
        }
    }
}