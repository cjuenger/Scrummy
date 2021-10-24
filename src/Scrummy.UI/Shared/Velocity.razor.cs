using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IO.Juenger.GitLab.Api;
using Microsoft.AspNetCore.Components;
using Scrummy.GitLab.Contracts.Parsers;
using Scrummy.GitLab.Parsers;
using Scrummy.Scrum.Contracts.Metrics;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.UI.Shared
{
    public partial class Velocity
    {
        private int _velocity;

        [Inject]
        private IVelocityCalculator VelocityCalculator { get; set; }
     
        [Inject]
        private IItemParser ItemParser { get; set; }
        
        [Parameter]
        public IEnumerable<Story> Stories { get; set; }

        [Parameter] 
        public int SprintLength { get; set; } = 1;

        [Parameter]
        public DateTime StartDate { get; set; }
        
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            
            if(Stories == null) return;
            
            _velocity = VelocityCalculator.GetVelocityPerSprint(Stories, StartDate, DateTime.Now, SprintLength);
        }
    }
}