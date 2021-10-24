﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IO.Juenger.GitLab.Api;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Scrummy.GitLab.Contracts.Parsers;
using Scrummy.GitLab.Parsers;
using Scrummy.Scrum.Contracts.Enums;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.UI.Shared
{
    public partial class StoriesOverview
    {
        [Inject]
        private IItemParser ItemParser { get; set; }
        
        [Parameter]
        public IEnumerable<Story> Stories { get; set; }
        
        private RadzenDataGrid<IGrouping<WorkflowState, Story>> _storiesGrid;
        private IEnumerable<IGrouping<WorkflowState, Story>> _groupedStories;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            _groupedStories = Stories?.GroupBy(s => s.State) 
                              ?? Enumerable.Empty<IGrouping<WorkflowState, Story>>();
        }
    }
}