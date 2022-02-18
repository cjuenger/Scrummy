using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Scrummy.DataAccess.Contracts.Enums;
using Scrummy.Scrum.Contracts.Enums;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.UI.Shared
{
    public partial class ItemPieChart
    {
        private ItemCategories[] _itemCategories;
        
        [Parameter]
        public string Title { get; set; }
        
        [Parameter] 
        public IEnumerable<Item> Items { get; set; }
        
        protected override void OnParametersSet(){
            
            base.OnParametersSetAsync();
            
            if(Items == null) return;

            var countOfTotalItems = Items.Count();
            var countOfStories = Items.OfType<Story>().Count();
            var countOfBugs = Items.Count(i => i.Type == ItemType.Bug);
            var countOfOthers = countOfTotalItems - countOfStories - countOfBugs;
            
            _itemCategories = new ItemCategories[]
            {
                new()
                {
                    Category = "Stories",
                    Count = countOfStories
                },
                new()
                {
                    Category = "Bugs",
                    Count = countOfBugs
                },
                new()
                {
                    Category = "Others",
                    Count = countOfOthers
                },
            };
        }
        
    }
    
    
    
    internal class ItemCategories
    {
        public string Category { get; set; }
        public int Count { get; set; }
    }
}