using Autofac;
using Scrummy.DataAccess.GitLab;
using Scrummy.Scrum;

namespace Io.Juenger.Scrummy.Service
{
    public class ScrummyServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterModules(builder);
        }
        
        private static void RegisterModules(ContainerBuilder builder)
        {
            builder.RegisterModule<GitLabModule>();
            builder.RegisterModule<ScrumModule>();
        }
    }
}