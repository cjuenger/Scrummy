using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.DataAccess.GitLab.Parsers;
using Scrummy.DataAccess.GitLab.Services;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.DataAccess.GitLab.Providers
{
    internal class ItemsProvider : IItemsProvider
    {
        private readonly IProjectApiProvider _projectApiProvider;
        private readonly IPaginationService _paginationService;
        private readonly IItemParser _itemParser;

        public ItemsProvider(
            IProjectApiProvider projectApiProvider,
            IPaginationService paginationService,
            IItemParser itemParser)
        {
            _projectApiProvider = projectApiProvider ?? throw new ArgumentNullException(nameof(projectApiProvider));
            _paginationService = paginationService ?? throw new ArgumentNullException(nameof(paginationService));
            _itemParser = itemParser ?? throw new ArgumentNullException(nameof(itemParser));
        }
        
        public async Task<IEnumerable<Item>> GetItemsOfProjectAsync(
            string projectId, 
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }

            var issues = await _paginationService
                .BrowseAllAsync(page => 
                    _projectApiProvider
                        .ProjectApi
                        .GetProjectIssuesAsync(projectId, page, cancellationToken: ct))
                .ConfigureAwait(false);

            var items = issues.Select(i => _itemParser.Parse(i));
            return items;
        }
        
        public async Task<IReadOnlyList<Item>> GetItemsOfSprintAsync(
            string projectId, 
            string sprintName, 
            CancellationToken ct = default)
        {
            var totalIssues = await _paginationService
                .BrowseAllAsync(page => 
                    _projectApiProvider
                        .ProjectApi
                        .GetProjectIssuesAsync(projectId, page, labels: new List<string> {sprintName}, cancellationToken: ct))
                .ConfigureAwait(false);
            
            var items = totalIssues.Select(i => _itemParser.Parse(i));

            return items.ToList();
        }
        
        public async Task<IEnumerable<Item>> GetItemsOfReleaseAsync(
            string projectId, 
            int releaseId, 
            CancellationToken ct = default)
        {
            var totalIssues=  
                await _paginationService
                    .BrowseAllAsync(page => 
                        _projectApiProvider
                            .ProjectApi
                            .GetAllIssuesOfProjectMilestoneAsync(projectId, releaseId, page, ct))
                    .ConfigureAwait(false);
            
            var items = totalIssues.Select(i => _itemParser.Parse(i));
            return items;
        }
    }
}