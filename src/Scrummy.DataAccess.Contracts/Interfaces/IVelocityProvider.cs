using System.Threading;
using System.Threading.Tasks;
using Scrummy.DataAccess.Contracts.Models;

namespace Scrummy.DataAccess.Contracts.Interfaces
{
    public interface IVelocityProvider
    {
        Velocity TotalAverageVelocity { get; }
        Velocity Best3AverageVelocity { get; }
        Velocity Worst3AverageVelocity { get; }
        Task LoadVelocityAsync(string projectId, CancellationToken ct = default);
    }
}