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
        private readonly IProjectApi _projectApi;
        private readonly IItemParser _itemParser;

        public ReleaseProvider(IProjectApi projectApi, IItemParser itemParser)
        {
            _projectApi = projectApi ?? throw new ArgumentNullException(nameof(projectApi));
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
            var issues = await _projectApi
                .GetAllIssuesOfProjectMilestoneAsync(projectId, releaseInfo.Id, ct)
                .ConfigureAwait(false);

            var items = issues.Select(i => _itemParser.Parse(i));
            return items;
        }
    }
}