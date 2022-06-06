using Io.Juenger.GitLabClient.Model;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.DataAccess.GitLab.Parsers
{
    internal interface IItemParser
    {
        Item Parse(Issue issue);
    }
}