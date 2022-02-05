using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IO.Juenger.GitLab.Api;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.DataAccess.Contracts.Models;
using Scrummy.DataAccess.GitLab.Parsers;

namespace Scrummy.DataAccess.GitLab.Providers
{
    internal class ReleaseProvider : IReleaseProvider
    {
        private readonly IProjectApiProvider _projectApiProvider;
        private readonly IItemParser _itemParser;

        public ReleaseProvider(IProjectApiProvider projectApiProvider, IItemParser itemParser)
        {
            _projectApiProvider = projectApiProvider ?? throw new ArgumentNullException(nameof(projectApiProvider));
            _itemParser = itemParser ?? throw new ArgumentNullException(nameof(itemParser));
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
            var issues = await _projectApiProvider.ProjectApi
                .GetAllIssuesOfProjectMilestoneAsync(projectId, releaseInfo.Id, ct)
                .ConfigureAwait(false);

            var items = issues.Select(i => _itemParser.Parse(i));
            return items;
        }
    }
}