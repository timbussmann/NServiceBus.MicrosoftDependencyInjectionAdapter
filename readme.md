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

Integrating with ASP.NET Core requires the usage of the `ServiceProviderFactory` as NServiceBus requires a working container when starting. The built container must then be returned from `ConfigureServices` so that both ASP.NET Core and NServiceBus use the same container.

```
// Return IServiceProvider
public IServiceProvider ConfigureServices(IServiceCollection services)
{
    // Register some services
    services.AddSingleton<ServiceA>(_ => new ServiceA("a"));

    // Configure the endpoint
    var ec = new EndpointConfiguration("Demo");
    ec.UseTransport<LearningTransport>();

    IServiceProvider serviceProvider = null;
    ec.UseContainer<MSDIAdapter>(c =>
    {
        c.UseServiceCollection(services);
        c.ServiceProviderFactory(sc =>
        {
            serviceProvider = sc.BuildServiceProvider();
            return serviceProvider;
        });
    });

    // You can still register services as long as the endpoint hasn't been started yet
    ec.RegisterComponents(c => c.RegisterSingleton(new ServiceB("b")));

    // Make IMessageSession available via DI
    IEndpointInstance endpointInstance = null;
    services.AddSingleton<IMessageSession>(_ => endpointInstance);
    
    // Start the endpoint
    endpointInstance = NServiceBus.Endpoint.Start(ec).GetAwaiter().GetResult();

    // Make sure to return the container here
    return serviceProvider;
}
```