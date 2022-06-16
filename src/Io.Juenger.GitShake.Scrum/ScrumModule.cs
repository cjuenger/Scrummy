using Autofac;
using Scrummy.Scrum.Contracts.Interfaces;
using Scrummy.Scrum.Metrics;
using Scrummy.Scrum.Services;

namespace Scrummy.Scrum
{
    public class ScrumModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<VelocityProvider>()
                .As<IVelocityProvider>()
                .SingleInstance();

            builder.RegisterType<ThroughputProvider>()
                .As<IThroughputProvider>()
                .SingleInstance();

            builder.RegisterType<VelocityCalculator>()
                .As<IVelocityCalculator>()
                .SingleInstance();
            
            builder.RegisterType<ChartProvider>()
                .As<IChartProvider>()
                .SingleInstance();

            builder
                .RegisterType<SprintMetrics>()
                .As<ISprintMetrics>()
                .SingleInstance();

            builder
                .RegisterType<BurnDownChartDataProvider>()
                .As<IBurnDownChartDataProvider>()
                .SingleInstance();
        }
    }
}