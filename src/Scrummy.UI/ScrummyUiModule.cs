using Autofac;
using Scrummy.DataAccess.Contracts.Models;
using Scrummy.DataAccess.GitLab;
using Scrummy.Scrum;
using Scrummy.UI.Configs;
using Scrummy.UI.Services;

namespace Scrummy.UI
{
    public class ScrummyUiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterTypes(builder);
            RegisterModules(builder);
        }

        private static void RegisterTypes(ContainerBuilder builder)
        {
            builder.RegisterType<GitLabConfig>()
                .As<IGitLabConfig>()
                .SingleInstance();

            builder.RegisterType<ScrumConfig>()
                .As<IScrumConfig>()
                .SingleInstance();

            builder.RegisterType<DragAndDropService>()
                .As<IDragAndDropService>();
            
            // builder.RegisterGeneric(typeof(DragAndDropService<>))
            //     .As(typeof(IDragAndDropService<>));
        }
        
        private static void RegisterModules(ContainerBuilder builder)
        {
            builder.RegisterModule<ScrumModule>();
            builder.RegisterModule<GitLabModule>();
        }
    }
}