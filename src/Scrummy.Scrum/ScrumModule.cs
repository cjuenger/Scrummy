using Autofac;
using Scrummy.Scrum.Contracts.Metrics;
using Scrummy.Scrum.Metrics;

namespace Scrummy.Scrum
{
    public class ScrumModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<VelocityCalculator>()
                .As<IVelocityCalculator>()
                .SingleInstance();
        }
    }
}