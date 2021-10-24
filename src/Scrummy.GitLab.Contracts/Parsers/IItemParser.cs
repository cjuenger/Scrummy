using IO.Juenger.GitLab.Model;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.GitLab.Contracts.Parsers
{
    public interface IItemParser
    {
        Item Parse(Issue issue);
    }
}