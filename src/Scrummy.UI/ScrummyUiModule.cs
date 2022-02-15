using Autofac;
using Scrummy.Scrum;
using Scrummy.UI.Configs;

namespace Scrummy.UI
{
    public class ScrummyUiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterTypes(builder);
            RegisterComponents(builder);
        }

        private static void RegisterTypes(ContainerBuilder builder)
        {
            builder.RegisterType<ScrumConfig>()
                .As<IScrumConfig>()
                .SingleInstance();
        }
        
        private static void RegisterComponents(ContainerBuilder builder)
        {
            var gitLabComponentActivator = new Scrummy.DataAccess.GitLab.ComponentActivator();
            gitLabComponentActivator.RegisterDependencies(builder);

            var scrumComponentActivator = new Scrummy.Scrum.ComponentActivator();
            scrumComponentActivator.RegisterDependencies(builder);
        }
    }
}