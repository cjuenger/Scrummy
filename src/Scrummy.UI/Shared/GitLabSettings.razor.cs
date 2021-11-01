﻿using Microsoft.AspNetCore.Components;
using Scrummy.UI.Configs;

namespace Scrummy.UI.Shared
{
    public partial class GitLabSettings
    {
        [Inject]
        private IGitLabConfig GitLabConfig { get; set; }
        
        private string ProjectId
        {
            get => GitLabConfig.ProjectId; 
            set => GitLabConfig.ProjectId = value;
        }
    }
}