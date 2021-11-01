using IO.Juenger.GitLab.Model;
using Scrummy.DataAccess.Contracts.Models;

namespace Scrummy.DataAccess.GitLab.Parsers
{
    internal interface IItemParser
    {
        Item Parse(Issue issue);
    }
}