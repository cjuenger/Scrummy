using System.Collections.Generic;
using Scrummy.Scrum.Contracts.Enums;

namespace Scrummy.GitLab.Contracts.Configs
{
    public interface IItemParserConfig
    {
        string StoryLabel { get; set; }
        
        string BugLabel { get; set; }
        
        string StoryPointPattern { get; set; }
        
        string StoryPointSplitter { get; set; }
        
        Dictionary<string, WorkflowState> LabelToWorkflowMapping { get; set; }
    }
}