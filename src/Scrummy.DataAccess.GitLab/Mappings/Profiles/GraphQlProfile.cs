using System;
using AutoMapper;
using Scrummy.DataAccess.Contracts.Enums;
using Scrummy.DataAccess.GitLab.GraphQl.Responses;
using Scrummy.DataAccess.GitLab.Parsers;
using Scrummy.Scrum.Contracts.Enums;
using Scrummy.Scrum.Contracts.Models;

namespace Scrummy.DataAccess.GitLab.Mappings.Profiles
{
    internal class GraphQlProfile : Profile
    {
        private readonly IGraphQlIssueParser _graphQlIssueParser;

        public GraphQlProfile(IGraphQlIssueParser graphQlIssueParser)
        {
            _graphQlIssueParser = graphQlIssueParser ?? throw new ArgumentNullException(nameof(graphQlIssueParser));
            
            CreateMap<Issue, Item>()
                .ForMember(
                    item => item.Title,
                    opt => 
                        opt.MapFrom(issue => issue.Title))
                .ForMember(
                    item => item.Type,
                    opt => 
                        opt.MapFrom(issue => GetItemType(issue)))
                .ForMember(
                    item => item.State,
                    opt => 
                        opt.MapFrom(issue => GetWorkflowState(issue)))
                .ForMember(
                    item => item.Description,
                    opt => 
                        opt.MapFrom(issue => issue.Description))
                .ForMember(
                    item => item.CreatedAt,
                    opt => 
                        opt.MapFrom(issue => issue.CreatedAt))
                .ForMember(
                    item => item.StartedAt,
                    opt => 
                        opt.Ignore())
                .ForMember(
                    item => item.ClosedAt,
                    opt => 
                        opt.MapFrom(issue => issue.ClosedAt))
                .ForMember(
                    item => item.Tasks,
                    opt => 
                        opt.Ignore())
                .ForMember(
                    item => item.Link,
                    opt => 
                        opt.MapFrom(issue => issue.WebUrl));
        }

        private static ItemType GetItemType(Issue issue)
        {
            throw new NotImplementedException();
        }

        private WorkflowState GetWorkflowState(Issue issue) => _graphQlIssueParser.GetWorkflowState(issue);
    }
}