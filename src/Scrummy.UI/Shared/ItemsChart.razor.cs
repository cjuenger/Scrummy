using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Scrummy.DataAccess.Contracts.Enums;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.UI.Shared
{
    public partial class ItemsChart
    {
        private int _countOfOpened = 0;
        private int _countOfClosed = 0;
        
        private IEnumerable<Xy<DateTime, int>> _opened;
        
        private IEnumerable<Xy<DateTime, int>> _closed;

        private readonly bool _smooth = false;
        
        [Parameter]
        public ItemType ItemType { get; set; }

        [Parameter]
        public IEnumerable<Item> Items { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            _opened = GetOpenedItemsSeries();
            _closed = GetClosedItemsSeries();
        }

        private IEnumerable<Xy<DateTime, int>> GetOpenedItemsSeries()
        {
             var opened = Items?
                .Where(i => i.Type == ItemType)
                .OrderBy(i => i.CreatedAt)
                .Select((i, idx) => new Xy<DateTime, int>
                {
                    X = i.CreatedAt,
                    Y = idx + 1
                }).ToList();
             
            if(opened == null) return Enumerable.Empty<Xy<DateTime, int>>();
            if (!opened.Any()) return opened;
            
            _countOfOpened = opened.Count;
            opened.Add(new Xy<DateTime, int> {X = DateTime.Now, Y = opened.Last().Y});

            return opened;
        }

        private IEnumerable<Xy<DateTime, int>> GetClosedItemsSeries()
        {
            var closed = Items?
                .Where(i => i.State == WorkflowState.Closed)
                .Where(i => i.Type == ItemType)
                .OrderBy(i => i.ClosedAt)
                .Where(i => i.ClosedAt != null)
                .Select((i, idx) => new Xy<DateTime, int>
                {
                    X = i.ClosedAt.Value,
                    Y = idx + 1
                }).ToList();
            
            if(closed == null) return Enumerable.Empty<Xy<DateTime, int>>();
            if (!closed.Any()) return closed;
            
            _countOfClosed = closed.Count;
            closed.Add(new Xy<DateTime, int> {X = DateTime.Now, Y = closed.Last().Y});

            return closed;
        }
    }
}