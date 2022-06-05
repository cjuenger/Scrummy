using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.DataAccess.GitLab.Caching;
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
        private readonly IMemoryCache _memoryCache;
        private readonly ICacheKeyBuilder _cacheKeyBuilder;

        public ItemsProvider(
            IProjectApiProvider projectApiProvider,
            IPaginationService paginationService,
            IItemParser itemParser,
            IMemoryCache memoryCache,
            ICacheKeyBuilder cacheKeyBuilder)
        {
            _projectApiProvider = projectApiProvider ?? throw new ArgumentNullException(nameof(projectApiProvider));
            _paginationService = paginationService ?? throw new ArgumentNullException(nameof(paginationService));
            _itemParser = itemParser ?? throw new ArgumentNullException(nameof(itemParser));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _cacheKeyBuilder = cacheKeyBuilder ?? throw new ArgumentNullException(nameof(cacheKeyBuilder));
        }
        
        public async Task<IEnumerable<Item>> GetItemsOfProjectAsync(
            string projectId, 
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }

            _cacheKeyBuilder.ProjectId = projectId;
            _cacheKeyBuilder.Key = nameof(IList<Item>);
            var key = _cacheKeyBuilder.Build();

            if (_memoryCache.TryGetValue(key, out IList<Item> cachedItems)) return cachedItems;
            
            var issues = await _paginationService
                .BrowseAllAsync(page => 
                    _projectApiProvider
                        .ProjectApi
                        .GetProjectIssuesAsync(projectId, page, cancellationToken: ct))
                .ConfigureAwait(false);
            
            var items = issues
                .Select(i => _itemParser.Parse(i))
                .ToList();

            _memoryCache.Set(key, items, TimeSpan.FromMinutes(5));
                
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