using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scrummy.DataAccess.Contracts.Models;

namespace Scrummy.DataAccess.Contracts.Providers
{
    public interface IBacklogProvider
    {
        Task<IEnumerable<Item>> GetItemsAsync(string projectPath, CancellationToken ct);
    }
}