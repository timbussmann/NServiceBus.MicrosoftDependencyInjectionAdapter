using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.ObjectBuilder.Common;

namespace NServiceBus.MicrosoftDependencyInjectionAdapter
{
    public class ConfigurableServiceCollectionAdapter : IContainer
    {
        private readonly IServiceCollection serviceCollection;
        private readonly bool externallyOwned;
        private readonly Lazy<IServiceProvider> serviceProvider;

        public ConfigurableServiceCollectionAdapter(IServiceCollection serviceCollection, Func<IServiceCollection, IServiceProvider> serviceProviderFactory, bool externallyOwned)
        {
            this.serviceCollection = serviceCollection;
            this.externallyOwned = externallyOwned;
            serviceProvider = new Lazy<IServiceProvider>(() => serviceProviderFactory(this.serviceCollection), LazyThreadSafetyMode.PublicationOnly);
           
        }

        public void Dispose()
        {
            if (externallyOwned && serviceProvider.Value is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        public object Build(Type typeToBuild)
        {
            return serviceProvider.Value.GetService(typeToBuild) ?? throw new Exception($"Unable to build {typeToBuild.FullName}. Ensure the type has been registered correctly with the container.");
        }

        public IContainer BuildChildContainer()
        {
            return new ChildServiceCollectionAdapter(serviceProvider.Value.CreateScope());
        }

        public IEnumerable<object> BuildAll(Type typeToBuild)
        {
            return serviceProvider.Value.GetServices(typeToBuild);
        }

        public void Configure(Type component, DependencyLifecycle dependencyLifecycle)
        {
            if (serviceCollection.Any(s => s.ServiceType == component))
            {
                return;
            }

            serviceCollection.Add(new ServiceDescriptor(component, component, Map(dependencyLifecycle)));
            RegisterInterfaces(component);
        }

        public void Configure<T>(Func<T> componentFactory, DependencyLifecycle dependencyLifecycle)
        {
            var componentType = typeof(T);
            if (serviceCollection.Any(s => s.ServiceType == componentType))
            {
                return;
            }

            serviceCollection.Add(new ServiceDescriptor(componentType, p => componentFactory(), Map(dependencyLifecycle)));
            RegisterInterfaces(componentType);
        }

        void RegisterInterfaces(Type component)
        {
            var interfaces = component.GetInterfaces();
            foreach (var serviceType in interfaces)
            {
                // see https://andrewlock.net/how-to-register-a-service-with-multiple-interfaces-for-in-asp-net-core-di/
                serviceCollection.Add(new ServiceDescriptor(serviceType, sp => sp.GetService(component), ServiceLifetime.Transient));
            }
        }

        public void RegisterSingleton(Type lookupType, object instance)
        {
            serviceCollection.AddSingleton(lookupType, instance);
        }

        public bool HasComponent(Type componentType)
        {
            return serviceCollection.Any(sd => sd.ServiceType == componentType);
        }

        public void Release(object instance)
        {
            throw new NotSupportedException();
        }

        static ServiceLifetime Map(DependencyLifecycle lifetime)
        {
            switch (lifetime)
            {
                case DependencyLifecycle.SingleInstance: return ServiceLifetime.Singleton;
                case DependencyLifecycle.InstancePerCall: return ServiceLifetime.Transient;
                case DependencyLifecycle.InstancePerUnitOfWork: return ServiceLifetime.Scoped;
                default: throw new NotSupportedException();
            }
        }
    }
}