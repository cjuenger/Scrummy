using System.Threading.Tasks;
using Scrummy.GitLab.GraphQl.Responses;

namespace Scrummy.GitLab.GraphQl.Queries
{
    public interface IBoardQueries
    {
        Task<Project> GetBoardsAsync(string projectPath);
        Task<Project> GetBoardWithListsAsync(string projectPath, string boardId);
    }
}