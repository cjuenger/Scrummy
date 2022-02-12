using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.DataAccess.Contracts.Interfaces
{
    public interface IBacklogProvider
    {
        Task<IEnumerable<Item>> GetItemsAsync(string projectPath, CancellationToken ct);
    }
}