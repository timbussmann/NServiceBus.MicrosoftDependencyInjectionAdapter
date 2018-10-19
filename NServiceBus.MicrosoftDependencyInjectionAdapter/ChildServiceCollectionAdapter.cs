using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.ObjectBuilder.Common;

namespace NServiceBus.MicrosoftDependencyInjectionAdapter
{
    class ChildServiceCollectionAdapter : IContainer
    {
        private readonly IServiceScope serviceScope;

        public ChildServiceCollectionAdapter(IServiceScope serviceScope)
        {
            this.serviceScope = serviceScope;
        }

        public void Dispose()
        {
            serviceScope.Dispose();
        }

        public object Build(Type typeToBuild)
        {
            return serviceScope.ServiceProvider.GetService(typeToBuild);
        }

        public IContainer BuildChildContainer()
        {
            throw new InvalidOperationException();
        }

        public IEnumerable<object> BuildAll(Type typeToBuild)
        {
            return serviceScope.ServiceProvider.GetServices(typeToBuild);
        }

        public void Configure(Type component, DependencyLifecycle dependencyLifecycle)
        {
            throw new InvalidOperationException();
        }

        public void Configure<T>(Func<T> component, DependencyLifecycle dependencyLifecycle)
        {
            throw new InvalidOperationException();
        }

        public void RegisterSingleton(Type lookupType, object instance)
        {
            throw new InvalidOperationException();
        }

        public bool HasComponent(Type componentType)
        {
            throw new NotSupportedException();
        }

        public void Release(object instance)
        {
            throw new NotSupportedException();
        }
    }
}