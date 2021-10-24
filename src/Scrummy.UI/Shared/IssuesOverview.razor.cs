using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Humanizer;
using IO.Juenger.GitLab.Api;
using IO.Juenger.GitLab.Client;
using IO.Juenger.GitLab.Model;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

// ReSharper disable once IdentifierTypo
namespace Scrummy.UI.Shared
{
    public partial class IssuesOverview
    {
        [Parameter]
        public IEnumerable<Issue> Issues { get; set; }
        
        [Inject]
        private IMapper Mapper { get; set; }

        private RadzenDataGrid<IGrouping<string, Issue>> _issuesGrid;
        private IEnumerable<IGrouping<string, Issue>> _issuesGroups;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            _issuesGroups = GroupIssuesByLabels(Issues).ToList();
        }

        private IEnumerable<IGrouping<string, Issue>> GroupIssuesByLabels(IEnumerable<Issue> issues)
        {
            var groupedIssues = issues?
                .SelectMany(DeconstructIssueByLabel)
                .GroupBy(i => i.Labels.First()) ?? Enumerable.Empty<IGrouping<string, Issue>>();

            return groupedIssues;
        }
        
        private IEnumerable<Issue> DeconstructIssueByLabel(Issue issue)
        {
            var labels = issue.Labels.ToList();
            if(!labels.Any()) labels.Add("");  
            
            var issueWithNoLabels = Mapper.Map<Issue>(issue);
            issueWithNoLabels.Labels.Clear();
            
            var issues = labels.Select(l =>
            {
                var issueWithSingleLabel = Mapper.Map<Issue>(issueWithNoLabels);
                issueWithSingleLabel.Labels.Add(l);
                
                return issueWithSingleLabel;
            });
            return issues;
        }
    }
}