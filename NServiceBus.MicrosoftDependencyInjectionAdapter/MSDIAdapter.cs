using System;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.Container;
using NServiceBus.ObjectBuilder.Common;
using NServiceBus.Settings;

namespace NServiceBus.MicrosoftDependencyInjectionAdapter
{
    public class MSDIAdapter : ContainerDefinition
    {
        public override IContainer CreateContainer(ReadOnlySettings settings)
        {
            ServiceCollection serviceCollection = null;
            if (!settings.TryGet(out serviceCollection))
            {
                serviceCollection = new ServiceCollection();
            }

            Func<IServiceCollection, IServiceProvider> factory;
            if (!settings.TryGet("MSDIAdapter.Factory", out factory))
            {
                // default factory:
                factory = sc => sc.BuildServiceProvider();
            }
            return new ConfigurableServiceCollectionAdapter(serviceCollection, factory);
        }
    }
}