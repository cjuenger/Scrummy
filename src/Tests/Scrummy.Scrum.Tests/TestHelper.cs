using System;
using System.Collections.Generic;
using Scrummy.DataAccess.Contracts.Enums;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Tests
{
    public class TestHelper
    {
        public static IReadOnlyList<Sprint> GetDummySprints()
        {
            var sprint1 = new Sprint
            (
                new SprintInfo
                {
                    StartTime = new DateTime(2021, 11, 22),
                    EndTime = new DateTime(2021, 12, 03)
                    
                },
                new List<Item>
                {
                    new Story { State = WorkflowState.Closed, StoryPoints = 13 },
                    new Story { State = WorkflowState.Closed, StoryPoints = 8 },
                    new Story { State = WorkflowState.Closed, StoryPoints = 5 },
                    new Story { State = WorkflowState.Closed, StoryPoints = 20 }
                }
            );
            
            var sprint2 = new Sprint
            (
                new SprintInfo
                {
                    StartTime = new DateTime(2021, 12, 06),
                    EndTime = new DateTime(2021, 12, 17)   
                },
                new List<Item>
                {
                    new Story { State = WorkflowState.Closed, StoryPoints = 1 },
                    new Story { State = WorkflowState.Closed, StoryPoints = 2 },
                    new Story { State = WorkflowState.Closed, StoryPoints = 3 },
                    new Story { State = WorkflowState.Closed, StoryPoints = 5 }
                }
            );
            
            var sprint3 = new Sprint
            (
                new SprintInfo
                {
                    StartTime = new DateTime(2021, 12, 20),
                    EndTime = new DateTime(2021, 12, 31)
                },
                new List<Item>
                {
                    new Story { State = WorkflowState.Closed, StoryPoints = 2 },
                    new Story { State = WorkflowState.Closed, StoryPoints = 3 },
                    new Story { State = WorkflowState.Closed, StoryPoints = 5 },
                    new Story { State = WorkflowState.Closed, StoryPoints = 8 }
                }
            );
            
            var sprint4 = new Sprint
            (
                new SprintInfo
                {
                    StartTime = new DateTime(2022, 01, 03),
                    EndTime = new DateTime(2022, 01, 14)
                },
                new List<Item>
                {
                    new Story { State = WorkflowState.Closed, StoryPoints = 3 },
                    new Story { State = WorkflowState.Closed, StoryPoints = 5 },
                    new Story { State = WorkflowState.Closed, StoryPoints = 8 },
                    new Story { State = WorkflowState.Closed, StoryPoints = 13 }
                }
            );
            
            var sprint5 = new Sprint
            (
                new SprintInfo
                {
                    StartTime = new DateTime(2022, 01, 17),
                    EndTime = new DateTime(2022, 01, 28)
                },
                new List<Item>
                {
                    new Story { State = WorkflowState.Closed, StoryPoints = 5 },
                    new Story { State = WorkflowState.Closed, StoryPoints = 8 },
                    new Story { State = WorkflowState.Closed, StoryPoints = 13 },
                    new Story { State = WorkflowState.Closed, StoryPoints = 20 }
                }
            );
            
            var sprint6 = new Sprint
            (
                new SprintInfo
                {
                    StartTime = new DateTime(2022, 01, 31),
                    EndTime = new DateTime(2022, 02, 11)
                },
                new List<Item>
                {
                    new Story { State = WorkflowState.Closed, StoryPoints = 8 },
                    new Story { State = WorkflowState.Closed, StoryPoints = 13 },
                    new Story { State = WorkflowState.Closed, StoryPoints = 20},
                    new Story { State = WorkflowState.Closed, StoryPoints = 40 }
                }
            );

            var sprints = new[] {sprint1, sprint2, sprint3, sprint4, sprint5, sprint6 };

            return sprints;
        }
    }
}