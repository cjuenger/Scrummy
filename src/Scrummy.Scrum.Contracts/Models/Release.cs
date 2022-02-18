using System;
using System.Collections.Generic;

namespace Scrummy.Scrum.Contracts.Models
{
    public class Release
    {
        public ReleaseInfo Info { get; }

        public IReadOnlyList<Item> Items { get; }

        public Release(ReleaseInfo info, IReadOnlyList<Item> items)
        {
            Info = info ?? throw new ArgumentNullException(nameof(info));
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }
    }
}