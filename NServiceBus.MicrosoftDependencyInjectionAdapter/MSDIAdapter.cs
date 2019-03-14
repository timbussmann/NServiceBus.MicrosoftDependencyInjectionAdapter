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
            if (!settings.TryGet(out IServiceCollection serviceCollection))
            {
                serviceCollection = new ServiceCollection();
            }

            if (!settings.TryGet("MSDIAdapter.Factory", out Func<IServiceCollection, IServiceProvider> factory))
            {
                factory = sc => sc.BuildServiceProvider();
            }

            return new ConfigurableServiceCollectionAdapter(serviceCollection, factory);
        }
    }
}