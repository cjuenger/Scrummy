using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Scrummy.Scrum.Contracts.Models;
using Scrummy.Scrum.Contracts.Providers;

namespace Scrummy.UI.Shared
{
    public partial class BurnUpChart
    {
        private IEnumerable<Xy<DateTime, int>> _burnUp;
        
        private readonly bool _smooth = false;
        
        [Inject]
        private IChartGenerator ChartGenerator { get; set; }
        
        [Parameter] 
        public IEnumerable<Story> Stories { get; set; }
        
        [Parameter]
        public DateTime StartDate { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            _burnUp = ChartGenerator.GetBurnUpChart(Stories);
        }
    }
}