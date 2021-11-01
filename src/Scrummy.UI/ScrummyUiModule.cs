using Autofac;
using Scrummy.DataAccess.GitLab;
using Scrummy.Scrum;

namespace Scrummy.UI
{
    public class ScrummyUiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterModules(builder);
        }

        private static void RegisterModules(ContainerBuilder builder)
        {
            builder.RegisterModule<ScrumModule>();
            builder.RegisterModule<GitLabModule>();
        }
    }
}