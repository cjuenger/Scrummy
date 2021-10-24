using IO.Juenger.GitLab.Model;
using Scrummy.Scrum.Contracts.Enums;

namespace Scrummy.GitLab.Contracts.Parsers
{
    public interface IIssueParser
    {
        WorkflowState GetWorkflowState(Issue issue);
    }
}