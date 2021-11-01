using System.Collections.Generic;
using Scrummy.DataAccess.Contracts.Enums;

namespace Scrummy.DataAccess.Contracts.Configs
{
    public interface IWorkflowMappingConfig
    {
        Dictionary<string, WorkflowState> LabelToWorkflowMapping { get; set; }
    }
}