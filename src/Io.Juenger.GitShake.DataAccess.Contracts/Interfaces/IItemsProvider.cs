﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.DataAccess.Contracts.Interfaces
{
    public interface IItemsProvider
    {
        Task<IEnumerable<Item>> GetItemsOfProjectAsync(string projectId, CancellationToken ct = default);

        Task<IReadOnlyList<Item>> GetItemsOfSprintAsync(
            string projectId, 
            SprintInfo sprintInfo, 
            CancellationToken ct = default);

        Task<IEnumerable<Item>> GetItemsOfReleaseAsync(
            string projectId, 
            ReleaseInfo releaseInfo, 
            CancellationToken ct = default);
    }
}