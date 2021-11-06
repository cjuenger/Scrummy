using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Scrummy.DataAccess.Contracts.Enums;
using Scrummy.DataAccess.Contracts.Models;

namespace Scrummy.UI.Shared
{
    public partial class BoardComponent
    {
        [Parameter] public string Title { get; set; }
        
        [Parameter] public List<Item> Items { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }

        public Item DragItem { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            Items = new List<Item>
            {
                new() {Title = "Mock Item 1", State = WorkflowState.Ready},
                new() {Title = "Mock Item 2", State = WorkflowState.Planned},
                new() {Title = "Mock Item 3", State = WorkflowState.Processing},
                new() {Title = "Mock Item 4", State = WorkflowState.Opened},
                new Story{Title = "Mock Story 1", State = WorkflowState.Opened},
                new Story{Title = "Mock Story 2", State = WorkflowState.Opened},
                new Story{Title = "Mock Story 3", State = WorkflowState.Opened},
                new Story{Title = "Mock Story 4", State = WorkflowState.Opened},
            };
            
            await base.OnInitializedAsync();
        }
    }
}