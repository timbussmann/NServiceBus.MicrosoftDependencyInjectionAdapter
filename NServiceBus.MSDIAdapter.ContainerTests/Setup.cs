using Microsoft.Extensions.DependencyInjection;
using NServiceBus.ContainerTests;
using NServiceBus.MicrosoftDependencyInjectionAdapter;
using NUnit.Framework;

[SetUpFixture]
public class Setup
{
    public Setup()
    {
        TestContainerBuilder.ConstructBuilder = () => new ConfigurableServiceCollectionAdapter(new ServiceCollection(), sc => sc.BuildServiceProvider(), false);
    }
}