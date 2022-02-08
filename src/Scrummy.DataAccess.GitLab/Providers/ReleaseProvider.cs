using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IO.Juenger.GitLab.Api;
using IO.Juenger.GitLab.Model;
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
            var page = 1;
            List<Issue> pagedIssues;
            var totalIssues = new List<Issue>();
            do
            {
                pagedIssues = await _projectApiProvider.ProjectApi
                    .GetAllIssuesOfProjectMilestoneAsync(projectId, releaseInfo.Id, page, ct)
                    .ConfigureAwait(false);
                
                totalIssues.AddRange(pagedIssues);
                page++;
            } 
            while (pagedIssues.Any());

            var items = totalIssues.Select(i => _itemParser.Parse(i));
            return items;
        }
    }
}