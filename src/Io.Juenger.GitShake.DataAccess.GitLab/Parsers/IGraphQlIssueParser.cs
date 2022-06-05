using Scrummy.DataAccess.Contracts.Enums;
using Scrummy.DataAccess.GitLab.GraphQl.Responses;

namespace Scrummy.DataAccess.GitLab.Parsers
{
    internal interface IGraphQlIssueParser
    {
        WorkflowState GetWorkflowState(Issue issue);
    }
}