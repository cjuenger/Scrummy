using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.DataAccess.GitLab.Parsers;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.DataAccess.GitLab.Providers
{
    internal class ReleaseProvider : IReleaseProvider
    {
        private readonly IProjectApiProvider _projectApiProvider;
        private readonly IItemParser _itemParser;
        private readonly IPaginationService _paginationService;

        public ReleaseProvider(
            IProjectApiProvider projectApiProvider, 
            IItemParser itemParser,
            IPaginationService paginationService)
        {
            _projectApiProvider = projectApiProvider ?? throw new ArgumentNullException(nameof(projectApiProvider));
            _itemParser = itemParser ?? throw new ArgumentNullException(nameof(itemParser));
            _paginationService = paginationService ?? throw new ArgumentNullException(nameof(paginationService));
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
                await GetAllItemsOfReleaseAsync(projectId, releaseInfo, ct)
                    .ConfigureAwait(false);

            var release = new Release(releaseInfo, items.ToList());
            return release;
        }
        
        private async Task<IEnumerable<Item>> GetAllItemsOfReleaseAsync(
            string projectId, 
            ReleaseInfo releaseInfo, 
            CancellationToken ct = default)
        {
            var totalIssues=  
                await _paginationService
                    .BrowseAllAsync(page => 
                        _projectApiProvider
                            .ProjectApi
                            .GetAllIssuesOfProjectMilestoneAsync(projectId, releaseInfo.Id, page, ct))
                    .ConfigureAwait(false);
            
            var items = totalIssues.Select(i => _itemParser.Parse(i));
            return items;
        }
    }
}