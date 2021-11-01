using System.Threading.Tasks;
using Scrummy.DataAccess.GitLab.GraphQl.Responses;

namespace Scrummy.DataAccess.GitLab.GraphQl.Queries
{
    internal interface IBoardQueries
    {
        Task<Project> GetBoardsAsync(string projectPath);
        Task<Project> GetBoardWithListsAsync(string projectPath, string boardId);
    }
}