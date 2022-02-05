using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.DataAccess.Contracts.Models;

namespace Scrummy.DataAccess.GitLab.Providers
{
    public class VelocityProvider : IVelocityProvider
    {
        private readonly ISprintProvider _sprintProvider;
        public Velocity TotalAverageVelocity { get; private set; }
        public Velocity Best3AverageVelocity { get; private set; }
        public Velocity Worst3AverageVelocity { get; private set; }

        public VelocityProvider(ISprintProvider sprintProvider)
        {
            _sprintProvider = sprintProvider ?? throw new ArgumentNullException(nameof(sprintProvider));
        }

        public async Task LoadVelocityAsync(string projectId, CancellationToken ct = default)
        {
            if (projectId == null) throw new ArgumentNullException(nameof(projectId));
            
            var sprints= await _sprintProvider
                .GetAllSprintsAsync(projectId, ct)
                .ConfigureAwait(false);
            
            CalculateVelocity(sprints);
        }

        private void CalculateVelocity(IEnumerable<Sprint> sprints)
        {
            var storyPoints = sprints
                .Select(sp => 
                    sp.Items
                        .OfType<Story>()
                        .Sum(st => st.StoryPoints ?? 0))
                .OrderBy(sp => sp)
                .ToList();

            TotalAverageVelocity = GetVelocity(storyPoints);
            Best3AverageVelocity = GetVelocity(storyPoints.TakeLast(3).ToList());
            Worst3AverageVelocity = GetVelocity(storyPoints.Take(3).ToList());
        }
        
        private static Velocity GetVelocity(IReadOnlyCollection<int> storyPoints)
        {
            var totalStoryPoints = storyPoints.Sum();
            
            var averageStoryPoints = totalStoryPoints / Math.Max(1,storyPoints.Count);
            
            var velocity = new Velocity
            {
                StoryPoints = averageStoryPoints,
                CountOfSprints = storyPoints.Count
            };

            return velocity;
        }
    }
}