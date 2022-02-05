using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Scrummy.DataAccess.Contracts.Models;

// ReSharper disable once IdentifierTypo
namespace Scrummy.UI.Shared
{
    public partial class ItemsOverview
    {
        [Parameter]
        public string Title { get; set; }
        
        [Parameter]
        public IEnumerable<Item> Items { get; set; }
        
        // protected override void OnParametersSet()
        // {
        //     base.OnParametersSet();
        // }
    }
}