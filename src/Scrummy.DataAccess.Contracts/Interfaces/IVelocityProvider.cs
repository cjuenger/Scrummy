using System;
using System.Threading;
using System.Threading.Tasks;

namespace Scrummy.DataAccess.Contracts.Interfaces
{
    public interface IVelocityProvider
    {
        float SprintAverageVelocity { get; }
        float Best3SprintsAverageVelocity { get; }
        float Worst3SprintsAverageVelocity { get; }
        float DayAverageVelocity { get; }
        float Best3SprintsDayAverageVelocity { get; }
        float Worst3SprintsDayAverageVelocity { get; }
        DateTime StartTimeOfFirstSprint { get; set; }
        DateTime EndTimeOfLastSprint { get; set; }
        Task LoadVelocityAsync(string projectId, CancellationToken ct = default);
    }
}