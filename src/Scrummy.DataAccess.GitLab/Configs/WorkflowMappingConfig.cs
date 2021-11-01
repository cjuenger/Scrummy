using System.Collections.Generic;
using Scrummy.DataAccess.Contracts.Configs;
using Scrummy.DataAccess.Contracts.Enums;

namespace Scrummy.DataAccess.GitLab.Configs
{
    internal class WorkflowMappingConfig : IWorkflowMappingConfig
    {
        public Dictionary<string, WorkflowState> LabelToWorkflowMapping { get; set; } = new()
        {
            {"Opened", WorkflowState.Opened},
            {"Ready", WorkflowState.Ready},
            {"Planned", WorkflowState.Planned},
            {"Processing", WorkflowState.Processing},
            {"Reviewing", WorkflowState.Reviewing},
            {"Testing", WorkflowState.Testing},
            {"Accepting", WorkflowState.Accepting},
            {"Closed", WorkflowState.Closed},
            {"Cancelled", WorkflowState.Cancelled},
        };
    }
}