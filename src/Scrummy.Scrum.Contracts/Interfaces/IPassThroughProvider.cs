using System.Threading;
using System.Threading.Tasks;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Contracts.Interfaces
{
    public interface IPassThroughProvider
    {
        Task<Throughput> GetPassThroughTimeAsync(string projectId, CancellationToken ct = default);
    }
}