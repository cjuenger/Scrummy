using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Scrummy.DataAccess.Contracts.Enums;
using Scrummy.DataAccess.Contracts.Models;

// ReSharper disable once IdentifierTypo
namespace Scrummy.UI.Shared
{
    public partial class ItemsOverview
    {
        [Parameter]
        public string Title { get; set; }

        private int _countOfStories;

        private int _countOfBugs;

        private int _countOfOthers;
        
        [Parameter]
        public IEnumerable<Item> Items { get; set; }
        
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if(Items == null) return;
            
            _countOfStories = Items.OfType<Story>().Count();
            _countOfBugs = Items.Count(i => i.Type == ItemType.Bug);
            _countOfOthers = Items.Count() - _countOfStories - _countOfBugs;
        }
    }
}