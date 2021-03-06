using Autofac;
using Io.Juenger.Common;
using Microsoft.Extensions.Configuration;

namespace Scrummy.Scrum
{
    /// <inheritdoc />
    public class ComponentActivator : IComponentActivator
    {
        /// <inheritdoc />
        public void Activate()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void Deactivate()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void RegisterDependencies(ContainerBuilder builder) => builder.RegisterModule<ScrumModule>();

        /// <inheritdoc />
        public void Configure(IConfiguration config)
        {
            throw new System.NotImplementedException();
        }
    }
}