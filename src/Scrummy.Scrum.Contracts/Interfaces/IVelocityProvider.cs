using System;
using System.Threading;
using System.Threading.Tasks;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Contracts.Interfaces
{
    public interface IVelocityProvider
    {
        public Velocity Velocity { get; }
        Task CalculateVelocityAsync(string projectId, DateTime endTime, CancellationToken ct = default);
        Task CalculateVelocityAsync(string projectId, CancellationToken ct = default);
    }
}