using Microsoft.Extensions.DependencyInjection;
using NServiceBus.ContainerTests;
using NServiceBus.MicrosoftDependencyInjectionAdapter;
using NUnit.Framework;

namespace NServiceBus.MSDIAdapter.ContainerTests
{
    [SetUpFixture]
    public class Setup
    {
        public Setup()
        {
            TestContainerBuilder.ConstructBuilder = () => new ConfigurableServiceCollectionAdapter(new ServiceCollection(), sc => sc.BuildServiceProvider());
        }
    }
}