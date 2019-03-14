using System;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.Container;

namespace NServiceBus.MicrosoftDependencyInjectionAdapter
{
    public static class ConfigurationExtensions
    {
        private const string FactoryKey = "MSDIAdapter.Factory";

        public static ContainerCustomizations UseServiceCollection(this ContainerCustomizations customizations, IServiceCollection serviceCollection)
        {
            customizations.Settings.Set<IServiceCollection>(serviceCollection);
            return customizations;
        }

        public static ContainerCustomizations ServiceProviderFactory(this ContainerCustomizations customizations,
            Func<IServiceCollection, IServiceProvider> providerFactory)
        {
            customizations.Settings.Set(FactoryKey, providerFactory);
            return customizations;
        }
    }
}