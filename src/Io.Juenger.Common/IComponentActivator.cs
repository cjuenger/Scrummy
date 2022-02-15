using Autofac;
using Microsoft.Extensions.Configuration;

namespace Io.Juenger.Common
{
    /// <summary>
    ///     A common interface that allows activating composite components
    /// </summary>
    public interface IComponentActivator
    {
        /// <summary>
        ///     Activates the component
        /// </summary>
        void Activate();
        
        /// <summary>
        ///     Deactivates the component
        /// </summary>
        void Deactivate();
        
        /// <summary>
        ///     Registers dependencies of the component to the DI container of the host
        /// </summary>
        /// <param name="builder">The DI container container builder</param>
        void RegisterDependencies(ContainerBuilder builder);
        
        /// <summary>
        ///     Configures the component
        /// </summary>
        /// <param name="config">Configuration</param>
        void Configure(IConfiguration config);
    }
}