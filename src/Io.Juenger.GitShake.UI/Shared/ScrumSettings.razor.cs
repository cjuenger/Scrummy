using System;
using Microsoft.AspNetCore.Components;
using Scrummy.UI.Configs;

namespace Scrummy.UI.Shared
{
    public partial class ScrumSettings
    {
        [Inject]
        private IScrumConfig ScrumConfig { get; set; }
        
        private DateTime? ProjectStart
        {
            get => ScrumConfig?.ProjectStart;
            set => ScrumConfig.ProjectStart = value ?? DateTime.MinValue;
        }

        private string SprintLength
        {
            get => ScrumConfig.SprintLength.ToString();
            set => ScrumConfig.SprintLength = int.Parse(value);
        }
    }
}