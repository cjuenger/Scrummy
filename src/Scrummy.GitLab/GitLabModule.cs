using Autofac;
using IO.Juenger.GitLab.Client;
using Scrummy.GitLab.Configs;
using Scrummy.GitLab.Contracts.Configs;
using Scrummy.GitLab.Contracts.Parsers;
using Scrummy.GitLab.GraphQl.Queries;
using Scrummy.GitLab.Parsers;
using Scrummy.GitLab.Providers;
using Scrummy.Scrum.Contracts.Providers;

namespace Scrummy.GitLab
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
            
            builder.RegisterType<BoardQueries>()
                .As<IBoardQueries>()
                .SingleInstance();

            builder.RegisterType<Configuration>()
                .As<IReadableConfiguration>()
                .SingleInstance();
        }
    }
}