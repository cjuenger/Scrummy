using Autofac;
using Scrummy.Scrum.Metrics;
using Scrummy.Scrum.Providers;
using IChartGenerator = Scrummy.Scrum.Providers.IChartGenerator;
using IVelocityCalculator = Scrummy.Scrum.Metrics.IVelocityCalculator;

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