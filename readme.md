This is a minimal implementation of a NServiceBus DI Container package supporting Microsoft.Extensions.DependencyInjection. It allows to use the default implementation but it can also wrap other containers.

The API doesn't provide extension methods to register existing ServiceCollections directly so they need to be set on the settings directly for now.

## Using the default IServiceProvider implementation:

```
// to configure your services using the ServiceCollection API:
var sc = new ServiceCollection();
sc.AddSingleton(new MyService());
endpointConfiguration.GetSettings().Set(sc);

endpointConfiguration.UseContainer<MSDIAdapter>();
```

## Using LightInject:

```
var lightinject = new LightInject.ServiceContainer();
lightinject.RegisterInstance(new MyService());
endpointConfiguration.GetSettings().Set("MSDIAdapter.Factory", (Func<IServiceCollection, IServiceProvider>)(sc => lightinject.CreateServiceProvider(sc)));

endpointConfiguration.UseContainer<MSDIAdapter>();
```

## Using DryIOC:

```
var dryIOC = new Container();
dryIOC.Register<MyService>(Reuse.Singleton);
endpointConfiguration.GetSettings().Set("MSDIAdapter.Factory", (Func<IServiceCollection, IServiceProvider>)(sc => dryIOC.WithDependencyInjectionAdapter(sc).BuildServiceProvider()));

endpointConfiguration.UseContainer<MSDIAdapter>();
```