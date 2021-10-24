using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.Scrum.Contracts.Providers
{
    public interface IBacklogProvider
    {
        Task<IEnumerable<Item>> GetItemsAsync(string projectPath, CancellationToken ct);
    }
}