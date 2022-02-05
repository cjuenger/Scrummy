using Autofac;
using AutoMapper;
using IO.Juenger.GitLab.Api;
using IO.Juenger.GitLab.Client;
using IO.Juenger.GitLab.Model;
using Microsoft.Extensions.Configuration;
using Scrummy.DataAccess.Contracts.Interfaces;
using Scrummy.DataAccess.Contracts.Models;
using Scrummy.DataAccess.GitLab.Configs;
using Scrummy.DataAccess.GitLab.GraphQl.Queries;
using Scrummy.DataAccess.GitLab.Parsers;
using Scrummy.DataAccess.GitLab.Providers;

namespace Scrummy.DataAccess.GitLab
{
    public class GitLabModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ItemParser>()
                .As<IItemParser>()
                .SingleInstance();

            builder.RegisterType<ItemParserConfig>()
                .As<IItemParserConfig>()
                .SingleInstance();

            builder.RegisterType<ItemsProvider>()
                .As<IItemsProvider>()
                .SingleInstance();

            builder.RegisterType<SprintProvider>()
                .As<ISprintProvider>()
                .SingleInstance();

            builder.RegisterType<SprintProviderConfig>()
                .As<ISprintProviderConfig>()
                .SingleInstance();
            
            builder.RegisterType<BacklogProvider>()
                .As<IBacklogProvider>()
                .SingleInstance();
            
            builder.RegisterType<BoardQueries>()
                .As<IBoardQueries>()
                .SingleInstance();

            builder.RegisterType<Configuration>()
                .As<IReadableConfiguration>()
                .SingleInstance();
            
            builder.Register(ctx => new ProjectApi(GetGitLabApiConfiguration(ctx)))
                .As<IProjectApi>()
                .SingleInstance();

            
            
            RegisterMappers(builder);
        }
        
        private static Configuration GetGitLabApiConfiguration(IComponentContext ctx)
        {
            var configuration = ctx.Resolve<IConfiguration>();
            var configurationSection = configuration.GetSection("GitLab");
            var basePath = configurationSection.GetValue("BasePath", "https://gitlab.com/api");
            var apiToken = configurationSection.GetValue<string>("AccessToken", null);
            var config = new Configuration { BasePath = basePath, AccessToken = apiToken };
            return config;
        }
        
        private static void RegisterMappers(ContainerBuilder builder)
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Issue, Issue>();
                cfg.CreateMap<Milestone, ReleaseInfo>();
            });
            var mapper = new Mapper(mapperConfig);
            builder.Register(_ => mapper).As<IMapper>();
        }
    }
}