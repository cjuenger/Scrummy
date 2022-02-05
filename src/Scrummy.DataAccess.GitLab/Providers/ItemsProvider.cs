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
    internal class ItemsProvider : IItemsProvider
    {
        private readonly IProjectApiProvider _projectApiProvider;
        private readonly IItemParser _itemParser;

        public ItemsProvider(
            IProjectApiProvider projectApiProvider, 
            IItemParser itemParser)
        {
            _projectApiProvider = projectApiProvider ?? throw new ArgumentNullException(nameof(projectApiProvider));
            _itemParser = itemParser ?? throw new ArgumentNullException(nameof(itemParser));
        }
        
        public async Task<IEnumerable<Item>> GetAllItemsAsync(
            string projectId, 
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectId));
            }
            
            var issues = await _projectApiProvider.ProjectApi
                .GetProjectIssuesAsync(projectId, cancellationToken: ct)
                .ConfigureAwait(false);

            var items = issues.Select(i => _itemParser.Parse(i));
            return items;
        }
    }
}