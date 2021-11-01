using System.Collections.Generic;
using Scrummy.DataAccess.Contracts.Enums;

namespace Scrummy.DataAccess.GitLab.Configs
{
    internal interface IItemParserConfig
    {
        string StoryLabel { get; }
        
        string BugLabel { get; }
        
        string StoryPointPattern { get; }
        
        string StoryPointSplitter { get; }
        
        Dictionary<string, WorkflowState> LabelToWorkflowMapping { get; }
    }
}