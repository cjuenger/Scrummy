using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IO.Juenger.GitLab.Api;
using Scrummy.GitLab.Contracts.Parsers;
using Scrummy.Scrum.Contracts.Models;
using Scrummy.Scrum.Contracts.Providers;

namespace Scrummy.GitLab.Providers
{
    public class ItemsProvider : IItemsProvider
    {
        private readonly string _projectId;
        private readonly IProjectApi _projectApi;
        private readonly IItemParser _itemParser;

        public ItemsProvider(
            string projectId,
            IProjectApi projectApi, 
            IItemParser itemParser)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }

            _projectId = projectId;
            _projectApi = projectApi ?? throw new ArgumentNullException(nameof(projectApi));
            _itemParser = itemParser ?? throw new ArgumentNullException(nameof(itemParser));
        }
        
        public async Task<IEnumerable<Item>> GetItemsAsync(CancellationToken ct)
        {
            var issues = await _projectApi
                .GetProjectIssuesAsync(_projectId, cancellationToken: ct)
                .ConfigureAwait(false);

            var items = issues.Select(i => _itemParser.Parse(i));
            return items;
        }
    }
}