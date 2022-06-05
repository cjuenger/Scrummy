using System;
using System.Threading;
using System.Threading.Tasks;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Contracts.Interfaces
{
    public interface IVelocityProvider
    {
        Task<Velocity> GetVelocityAsync(string projectId, DateTime endTime, CancellationToken ct = default);
        Task<Velocity> GetVelocityAsync(string projectId, CancellationToken ct = default);
    }
}