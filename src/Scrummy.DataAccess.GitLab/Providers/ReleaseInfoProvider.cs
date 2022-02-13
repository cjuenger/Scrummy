using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IO.Juenger.GitLab.Model;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.DataAccess.GitLab.Providers
{
    internal class ReleaseInfoProvider : IReleaseInfoProvider
    {
        private readonly IProjectApiProvider _projectApiProvider;
        private readonly IPaginationService _paginationService;
        private readonly IMapper _mapper;

        public ReleaseInfoProvider(
            IProjectApiProvider projectApiProvider, 
            IPaginationService paginationService,
            IMapper mapper)
        {
            _projectApiProvider = projectApiProvider ?? throw new ArgumentNullException(nameof(projectApiProvider));
            _paginationService = paginationService ?? throw new ArgumentNullException(nameof(paginationService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<(bool IsSuccess, ReleaseInfo ReleaseInfo)> TryGetNextUpcomingReleaseAsync(string projectId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }
            
            var projectApi = _projectApiProvider.ProjectApi;
            var activeMilestones = await _paginationService
                .BrowseAllAsync(page => 
                    projectApi
                        .GetProjectMilestonesAsync(projectId, page: page, cancellationToken: ct))
                .ConfigureAwait(false);

            var orderedActiveMilestones = activeMilestones.OrderBy(m => m.DueDate);

            var now = DateTime.UtcNow;
            var nextUpcomingMilestone =
                orderedActiveMilestones.FirstOrDefault(m => m.DueDate != null && now < m.DueDate);

            if (nextUpcomingMilestone == null) return (false, null);
            
            var nextUpcomingRelease = _mapper.Map<ReleaseInfo>(nextUpcomingMilestone);

            return (true, nextUpcomingRelease);
        }
        
        public async Task<IReadOnlyList<ReleaseInfo>> GetAllReleasesAsync(string projectId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }

            var projectApi = _projectApiProvider.ProjectApi;

            var milestones = await _paginationService
                .BrowseAllAsync(page => 
                    projectApi
                        .GetProjectMilestonesAsync(projectId, page: page, cancellationToken: ct))
                .ConfigureAwait(false);

            var releaseInfos = _mapper.Map<IEnumerable<Milestone>, List<ReleaseInfo>>(milestones);

            return releaseInfos;
        }
    }
}