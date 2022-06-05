using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Scrummy.DataAccess.Contracts.Enums;
using Scrummy.DataAccess.GitLab.Parsers;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.UI.Shared
{
    public partial class StoriesOverview
    {
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