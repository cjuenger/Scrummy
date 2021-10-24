using Scrummy.GitLab.GraphQl.Responses;
using Scrummy.Scrum.Contracts.Enums;

namespace Scrummy.GitLab.Parsers
{
    public interface IGraphQlIssueParser
    {
        WorkflowState GetWorkflowState(Issue issue);
    }
}