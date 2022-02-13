using Autofac;
using Scrummy.Scrum.Contracts.Interfaces;
using Scrummy.Scrum.Metrics;
using Scrummy.Scrum.Providers;
using Scrummy.Scrum.Services;
using IVelocityCalculator = Scrummy.Scrum.Contracts.Interfaces.IVelocityCalculator;

namespace Scrummy.Scrum
{
    public class ScrumModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<VelocityCalculator>()
                .As<IVelocityCalculator>()
                .SingleInstance();
            
            builder.RegisterType<VelocityProvider>()
                .As<IVelocityProvider>()
                .SingleInstance();
            
            builder.RegisterType<ChartGeneratorService>()
                .As<IChartGeneratorService>()
                .SingleInstance();
        }
    }
}