using Autofac;
using AutoMapper;
using IO.Juenger.GitLab.Api;
using IO.Juenger.GitLab.Client;
using IO.Juenger.GitLab.Model;
using Microsoft.Extensions.Configuration;
using Scrummy.GitLab;
using Scrummy.Scrum;

namespace Scrummy.UI
{
    public class ScrummyUiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterModules(builder);
            RegisterMappers(builder);
        }

        private static void RegisterModules(ContainerBuilder builder)
        {
            builder.RegisterModule<ScrumModule>();
            RegisterGitLabModule(builder);
        }
        
        private static void RegisterGitLabModule(ContainerBuilder builder)
        {
            builder.RegisterModule<GitLabModule>();
            builder.Register(ctx => new ProjectApi(GetGitLabApiConfiguration(ctx)))
                .As<IProjectApi>();
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
            var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<Issue, Issue>());
            var mapper = new Mapper(mapperConfig);
            builder.Register(ctx => mapper).As<IMapper>();
        }
    }
}