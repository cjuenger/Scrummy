using Autofac;
using Scrummy.Scrum.Contracts.Interfaces;
using Scrummy.Scrum.Providers;
using Scrummy.Scrum.Services;

namespace Scrummy.Scrum
{
    internal class ScrumModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<VelocityProvider>()
                .As<IVelocityProvider>()
                .SingleInstance();
            
            builder.RegisterType<ChartService>()
                .As<IChartService>()
                .SingleInstance();
        }
    }
}