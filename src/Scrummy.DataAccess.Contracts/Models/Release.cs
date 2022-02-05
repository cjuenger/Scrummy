using System.Collections.Generic;

namespace Scrummy.DataAccess.Contracts.Models
{
    public class Release
    {
        public ReleaseInfo Info { get; }

        public IReadOnlyList<Item> Items { get; }

        public Release(ReleaseInfo info, IReadOnlyList<Item> items)
        {
            Info = info;
            Items = items;
        }
    }
}