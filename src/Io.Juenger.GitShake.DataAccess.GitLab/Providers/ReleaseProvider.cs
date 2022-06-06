using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Io.Juenger.GitLabClient.Model;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.DataAccess.GitLab.Services;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.DataAccess.GitLab.Providers
{
    internal class ReleaseProvider : IReleaseProvider
    {
        private readonly IProjectApiProvider _projectApiProvider;
        private readonly IItemsProvider _itemsProvider;
        private readonly IPaginationService _paginationService;
        private readonly IMapper _mapper;

        public ReleaseProvider(
            IProjectApiProvider projectApiProvider, 
            IItemsProvider itemsProvider,
            IPaginationService paginationService,
            IMapper mapper)
        {
            _projectApiProvider = projectApiProvider ?? throw new ArgumentNullException(nameof(projectApiProvider));
            _itemsProvider = itemsProvider ?? throw new ArgumentNullException(nameof(itemsProvider));
            _paginationService = paginationService ?? throw new ArgumentNullException(nameof(paginationService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<Release> GetReleaseAsync(
            string projectId, 
            ReleaseInfo releaseInfo, 
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }

            var items = 
                await _itemsProvider.GetItemsOfReleaseAsync(projectId, releaseInfo.Id, ct)
                    .ConfigureAwait(false);

            var release = new Release(releaseInfo, items.ToList());
            return release;
        }
        
        public async Task<(bool IsSuccess, ReleaseInfo ReleaseInfo)> TryGetNextUpcomingReleaseAsync(string projectId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }

            var milestones = await GetMilestonesAsync(projectId, ct);

            var orderedActiveMilestones = milestones.OrderBy(m => m.DueDate);

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

            var milestones = await GetMilestonesAsync(projectId, ct);

            var releaseInfos = _mapper.Map<IEnumerable<Milestone>, List<ReleaseInfo>>(milestones);

            return releaseInfos;
        }

        private async Task<IEnumerable<Milestone>> GetMilestonesAsync(string projectId, CancellationToken ct)
        {
            var projectApi = _projectApiProvider.ProjectApi;

            var milestones = await _paginationService
                .BrowseAllAsync(page => 
                    projectApi
                        .GetProjectMilestonesAsync(projectId, page: page, cancellationToken: ct))
                .ConfigureAwait(false);

            return milestones;
        }
    }
}