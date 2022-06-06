using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Io.Juenger.GitLabClient.Model;
using Scrummy.DataAccess.Contracts.Enums;
using Scrummy.DataAccess.GitLab.Configs;
using Scrummy.Scrum.Contracts.Enums;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.DataAccess.GitLab.Parsers
{
    internal class ItemParser : IItemParser
    {
        private readonly IItemParserConfig _config;
        private readonly IDictionary<string, WorkflowState> _stateMap;

        public ItemParser(IItemParserConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            _stateMap = _config.LabelToWorkflowMapping;
        }
        
        public Item Parse(Issue issue)
        {
            Item item;

            var state = GetItemState(issue);
            
            if (IsStory(issue))
            {
                var hasStoryPoints = TryGetStoryPoints(issue, out var storyPoints);

                item = new Story
                {   
                    Type = ItemType.Story,
                    Title = issue.Title,
                    StoryPoints = hasStoryPoints ? storyPoints : null,
                    Description = issue.Description,
                    CreatedAt = issue.CreatedAt,
                    ClosedAt = issue.ClosedAt,
                    UpdatedAt = issue.UpdatedAt,
                    State = state
                };
            }
            else
            {
                item = new Item
                {
                    Type = IsBug(issue) ? ItemType.Bug : ItemType.Other,
                    Title = issue.Title,
                    Description = issue.Description,
                    CreatedAt = issue.CreatedAt,
                    ClosedAt = issue.ClosedAt,
                    UpdatedAt = issue.UpdatedAt,
                    State = state
                };
            }

            return item;
        }

        private bool TryGetStoryPoints(Issue issue, out int storyPoints)
        {
            storyPoints = 0;
            var storyPointLabel = issue.Labels.FirstOrDefault(IsStoryPointLabel);
            if (storyPointLabel == null) return false;
            
            storyPoints = GetStoryPoints(storyPointLabel);
            return true;
        }

        private bool IsStory(Issue issue) => issue.Labels.Contains(_config.StoryLabel);

        private bool IsBug(Issue issue) => issue.Labels.Contains(_config.BugLabel);
        
        private bool IsStoryPointLabel(string label) 
        {
            var rgx = new Regex(_config.StoryPointPattern);
            var match = rgx.Match(label);
            return match.Success;
        }
        
        private int GetStoryPoints(string label)
        {
            var split = label.Split(_config.StoryPointSplitter);
            return Convert.ToInt32(split[0]);
        }

        private WorkflowState GetItemState(Issue issue)
        {
            var state = WorkflowState.Opened;

            if (issue.ClosedAt != null)
            {
                state = WorkflowState.Closed;
                return state;
            } 
            
            var label = issue.Labels
                .Intersect(_stateMap.Keys)
                .FirstOrDefault();

            return label == null ? state : _stateMap[label];
        }
    }
}