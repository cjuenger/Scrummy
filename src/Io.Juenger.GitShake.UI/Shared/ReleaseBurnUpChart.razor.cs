using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Scrummy.Scrum.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Models;
using Scrummy.Scrum.Providers;

namespace Scrummy.UI.Shared
{
    public partial class ReleaseBurnUpChart
    {
        private IEnumerable<Xy<DateTime, int>> _burnUp;
        
        private readonly bool _smooth = false;
        
        [Inject]
        private IChartProvider ChartProvider { get; set; }
        
        [Parameter] 
        public IEnumerable<Story> Stories { get; set; }
        
        [Parameter]
        public DateTime StartDate { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            _burnUp = ChartProvider.GetBurnUpChart(Stories);
        }
    }
}