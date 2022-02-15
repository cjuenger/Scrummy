using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Scrummy.DataAccess.Contracts.Enums;
using Scrummy.Scrum.Contracts.Models;

// ReSharper disable once IdentifierTypo
namespace Scrummy.UI.Shared
{
    public partial class DetailedItemsOverview
    {
        [Parameter]
        public IEnumerable<Item> Items { get; set; }
        
        [Inject]
        private IMapper Mapper { get; set; }

        private RadzenDataGrid<IGrouping<ItemType, Item>> _itemsGrid;
        private IEnumerable<IGrouping<ItemType, Item>> _itemsGroups;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            _itemsGroups = GroupItemsByState(Items).ToList();
        }

        private static IEnumerable<IGrouping<ItemType, Item>> GroupItemsByState(IEnumerable<Item> items)
        {
            var groupedIssues = items?
                .GroupBy(i => i.Type) ?? Enumerable.Empty<IGrouping<ItemType, Item>>();

            return groupedIssues;
        }
    }
}