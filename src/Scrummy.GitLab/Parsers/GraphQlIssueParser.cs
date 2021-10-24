using System;
using System.Linq;
using Scrummy.GitLab.Contracts.Configs;
using Scrummy.GitLab.GraphQl.Responses;
using Scrummy.Scrum.Contracts.Enums;

namespace Scrummy.GitLab.Parsers
{
    public class GraphQlIssueParser : IGraphQlIssueParser
    {
        private readonly IItemParserConfig _config;

        public GraphQlIssueParser(IItemParserConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }
        
        public WorkflowState GetWorkflowState(Issue issue)
        {
            var state = WorkflowState.Opened;

            if (issue.ClosedAt != null)
            {
                state = WorkflowState.Closed;
                return state;
            } 
            
            var label = issue.Labels.LabelsEdges
                .Select(le => le.Label.Title)
                .Intersect(_config.LabelToWorkflowMapping.Keys)
                .FirstOrDefault();

            return label == null ? state : _config.LabelToWorkflowMapping[label];
        }
    }
}