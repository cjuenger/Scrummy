using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IO.Juenger.GitLab.Api;
using IO.Juenger.GitLab.Model;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.DataAccess.Contracts.Models;

namespace Scrummy.DataAccess.GitLab.Providers
{
    internal class ReleaseInfoProvider : IReleaseInfoProvider
    {
        private readonly IProjectApi _projectApi;
        private readonly IMapper _mapper;

        public ReleaseInfoProvider(IProjectApi projectApi, IMapper mapper)
        {
            _projectApi = projectApi ?? throw new ArgumentNullException(nameof(projectApi));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<(bool IsSuccess, ReleaseInfo ReleaseInfo)> TryGetNextUpcomingReleaseAsync(string projectId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }
            
            var activeMilestones = await _projectApi
                .GetProjectMilestonesAsync(projectId, state: MilestoneState.Active, cancellationToken: ct)
                .ConfigureAwait(false);

            var orderedActiveMilestones = activeMilestones.OrderBy(m => m?.DueDate ?? default);

            var now = DateTime.UtcNow;
            var nextUpcomingMilestone =
                orderedActiveMilestones.FirstOrDefault(m => m.DueDate != null && now < m.DueDate);

            var nextUpcomingRelease = _mapper.Map<ReleaseInfo>(nextUpcomingMilestone);

            return (true, nextUpcomingRelease);
        }
        
        public async Task<IReadOnlyList<ReleaseInfo>> GetAllReleasesAsync(string projectId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }

            var milestones = await _projectApi
                .GetProjectMilestonesAsync(projectId, cancellationToken: ct)
                .ConfigureAwait(false);

            var releaseInfos = _mapper.Map<List<Milestone>, List<ReleaseInfo>>(milestones);

            return releaseInfos;
        }
    }
}