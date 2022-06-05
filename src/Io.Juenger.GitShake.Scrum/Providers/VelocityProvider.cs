using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Providers
{
    internal class VelocityProvider : IVelocityProvider
    {
        private readonly ISprintProvider _sprintProvider;
        private readonly IVelocityCalculator _velocityCalculator;
        
        public VelocityProvider(ISprintProvider sprintProvider, IVelocityCalculator velocityCalculator)
        {
            _sprintProvider = sprintProvider ?? throw new ArgumentNullException(nameof(sprintProvider));
            _velocityCalculator = velocityCalculator ?? throw new ArgumentNullException(nameof(velocityCalculator));
        }

        public async Task<Velocity> GetVelocityAsync(string projectId, DateTime endTime, CancellationToken ct = default)
        {
            if (projectId == null) throw new ArgumentNullException(nameof(projectId));
            
            var sprints= await _sprintProvider
                .GetAllSprintsAsync(projectId, ct)
                .ConfigureAwait(false);

            var sprintsWithStoryPoints = sprints
                .Where(sp => sp.Info.EndTime <= endTime)
                .Where(sp => sp.CompletedStoryPoints > 0)
                .ToList();

            var velocity = _velocityCalculator.CalculateVelocity(sprintsWithStoryPoints);
            return velocity;
        }

        public Task<Velocity> GetVelocityAsync(string projectId, CancellationToken ct = default)
        {
            return GetVelocityAsync(projectId, DateTime.UtcNow, ct);
        }
    }
}