This is a minimal implementation of a NServiceBus DI Container package supporting Microsoft.Extensions.DependencyInjection. It allows to use the default implementation but it can also wrap other containers.

## using the Adapter with the default IServiceProvider implementation:
`endpointConfiguration.UseContainer<MSDIAdapter>();`


## Providing an existing IServiceCollection:

```
// to configure your services using the ServiceCollection API:
var sc = new ServiceCollection();
sc.AddSingleton(new MyService());

endpointConfiguration.UseContainer<MSDIAdapter>(c => c.UseServiceCollection(sc));
```


## Using LightInject:

```
var lightinject = new LightInject.ServiceContainer();
lightinject.RegisterInstance(new MyService());

endpointConfiguration.UseContainer<MSDIAdapter>(c =>
{
    c.UseServiceCollection(lightinject);
    c.ServiceProviderFactory(lightinject.CreateServiceProvider(sc)));
}
 
```

## Using Lamar

```
var registry = new ServiceRegistry();
registry.For<MyService>().Use<MyService>().Singleton();

ec.UseContainer<MSDIAdapter>(c =>
{
    c.UseServiceCollection(registry);
    c.ServiceProviderFactory(sc => new Container(sc));
});
```

## Integrating with ASP.NET Core

Integrating with ASP.NET Core requires the usage of the `ServiceProviderFactory` as NServiceBus needs to use a usable container when starting. The built container must then be returned from `ConfigureServices` so that both ASP.NET use the same container.

```
public IServiceProvider ConfigureServices(IServiceCollection services)
{
    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
    services.AddSingleton<ServiceA>(_ => new ServiceA("a"));

    var ec = new EndpointConfiguration("Demo");
    ec.UseTransport<LearningTransport>();

    IServiceProvider serviceProvider = null;
    ec.UseContainer<MSDIAdapter>(c =>
    {
        c.UseServiceCollection(services);
        c.ServiceProviderFactory(sc =>
        {
            sc.AddSingleton<ServiceC>();
            serviceProvider = new Container(sc);
            return serviceProvider;
        });
    });

    ec.RegisterComponents(c => c.RegisterSingleton(new ServiceB("b")));

    IEndpointInstance endpointInstance = null;
    services.AddSingleton<IMessageSession>(_ => endpointInstance);
    endpointInstance = NServiceBus.Endpoint.Start(ec).GetAwaiter().GetResult();

    // make sure to return the container here
    return serviceProvider;
}
```