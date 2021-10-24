using System.Collections.Generic;
using Scrummy.Scrum.Contracts.Enums;

namespace Scrummy.GitLab.Contracts.Configs
{
    public interface IWorkflowMappingConfig
    {
        Dictionary<string, WorkflowState> LabelToWorkflowMapping { get; set; }
    }
}