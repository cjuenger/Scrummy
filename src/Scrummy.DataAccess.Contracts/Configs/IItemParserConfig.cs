using System.Collections.Generic;
using Scrummy.DataAccess.Contracts.Enums;

namespace Scrummy.DataAccess.Contracts.Configs
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