using IO.Juenger.GitLab.Model;
using Scrummy.DataAccess.Contracts.Enums;

namespace Scrummy.DataAccess.Contracts.Parsers
{
    public interface IIssueParser
    {
        WorkflowState GetWorkflowState(Issue issue);
    }
}