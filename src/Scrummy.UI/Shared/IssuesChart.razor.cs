using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IO.Juenger.GitLab.Api;
using IO.Juenger.GitLab.Model;
using Microsoft.AspNetCore.Components;
using Scrummy.UI.Models;

namespace Scrummy.UI.Shared
{
    public partial class IssuesChart
    {
        private IEnumerable<Xy<DateTime, int>> _opened;
        
        private IEnumerable<Xy<DateTime, int>> _closed;

        private readonly bool _smooth = false;
        
        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public IEnumerable<Issue> Issues { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            _opened = GetOpenedIssuesSeries();
            _closed = GetClosedIssuesSeries();
        }

        private bool FilterForLabel(Issue issue) => string.IsNullOrWhiteSpace(Label) || issue.Labels.Contains(Label);

        private IEnumerable<Xy<DateTime, int>> GetOpenedIssuesSeries()
        {
             var opened = Issues?
                .Where(FilterForLabel)
                .OrderBy(i => i.CreatedAt)
                .Select((i, idx) => new Xy<DateTime, int>
                {
                    X = i.CreatedAt,
                    Y = idx + 1
                }).ToList();
            
             if(opened == null) return Enumerable.Empty<Xy<DateTime, int>>();
             
            if (opened.Any() /*&& _closed?.Count() < _opened?.Count()*/)
            {
                opened.Add(new Xy<DateTime, int> {X = DateTime.Now, Y = opened.Last().Y});
            }

            return opened;
        }

        private IEnumerable<Xy<DateTime, int>> GetClosedIssuesSeries()
        {
            var closed = Issues?
                .Where(i => i.State == State.Closed)
                .Where(FilterForLabel)
                .OrderBy(i => i.ClosedAt)
                .Where(i => i.ClosedAt != null)
                .Select((i, idx) => new Xy<DateTime, int>
                {
                    X = i.ClosedAt.Value,
                    Y = idx + 1
                }).ToList();
            
            if(closed == null) return Enumerable.Empty<Xy<DateTime, int>>();

            if (closed.Any() /*&& _closed?.Count() < _opened?.Count()*/)
            {
                closed.Add(new Xy<DateTime, int> {X = DateTime.Now, Y = closed.Last().Y});
            }

            return closed;
        }
    }
}