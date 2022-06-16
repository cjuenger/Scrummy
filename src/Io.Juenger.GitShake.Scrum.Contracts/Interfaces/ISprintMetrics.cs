using System.Threading;
using System.Threading.Tasks;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Contracts.Interfaces
{
    public interface ISprintMetrics
    {
        Task<SprintComposition> GetSprintCompositionAsync(
            string projectId, 
            int sprintId,
            CancellationToken ct = default);
    }
}