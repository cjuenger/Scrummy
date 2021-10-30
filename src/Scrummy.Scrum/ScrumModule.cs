using Autofac;
using Scrummy.Scrum.Contracts.Metrics;
using Scrummy.Scrum.Contracts.Providers;
using Scrummy.Scrum.Metrics;
using Scrummy.Scrum.Providers;

namespace Scrummy.Scrum
{
    public class ScrumModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<VelocityCalculator>()
                .As<IVelocityCalculator>()
                .SingleInstance();
            
            builder.RegisterType<ChartGenerator>()
                .As<IChartGenerator>()
                .SingleInstance();
        }
    }
}