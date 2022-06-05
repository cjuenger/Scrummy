using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.UI.Shared
{
    public partial class AgedItemsOverview
    {
        [Inject]
        private IDataAccessConfig DataAccessConfig { get; set; }
        
        [Inject]
        private IItemsProvider ItemsProvider { get; set; }
        
        private string Title { get; set; }
        
        private IEnumerable<Item> AgedItems { get; set; }

        [Parameter] 
        public int LateItemDaysThreshold { get; set; }
        
        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync().ConfigureAwait(false);
            
            var items = await ItemsProvider
                .GetItemsOfProjectAsync(DataAccessConfig.ProjectId)
                .ConfigureAwait(false);

            AgedItems = items.Where(item =>
            {
                if (item.ClosedAt != null) return false;
                var idleTime = DateTime.UtcNow - item.UpdatedAt;
                var isAged = idleTime.TotalDays >= 60;
                return isAged;
            });

            Title = $"Items not touched for {LateItemDaysThreshold} days or more";
            
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);
        }
    }
}