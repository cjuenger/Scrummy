﻿using System;
using System.Linq;
using IO.Juenger.GitLab.Model;
using Scrummy.GitLab.Contracts.Configs;
using Scrummy.GitLab.Contracts.Parsers;
using Scrummy.Scrum.Contracts.Enums;

namespace Scrummy.GitLab.Parsers
{
    public class IssueParser : IIssueParser
    {
        private readonly IItemParserConfig _config;

        public IssueParser(IItemParserConfig config)
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
            
            var label = issue.Labels
                .Intersect(_config.LabelToWorkflowMapping.Keys)
                .FirstOrDefault();

            return label == null ? state : _config.LabelToWorkflowMapping[label];
        }
    }
}