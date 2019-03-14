using System;
using System.Threading.Tasks;
using Lamar;
using NServiceBus;
using NServiceBus.MicrosoftDependencyInjectionAdapter;

namespace Demo
{
    public class Program
    {
        public static async Task Main()
        {
            var ec = new EndpointConfiguration("MSDIAdapterDemo");

            var registry = new ServiceRegistry();
            registry.For<ServiceA>().Use<ServiceA>().Singleton();

            ec.UseContainer<MSDIAdapter>(c =>
            {
                c.UseServiceCollection(registry);
                c.ServiceProviderFactory(sc => new Container(sc));
            });

            ec.UseTransport<LearningTransport>();

            ec.RegisterComponents(c => c.ConfigureComponent(typeof(ServiceB), DependencyLifecycle.SingleInstance));

            var endpoint = await Endpoint.Start(ec);
            await endpoint.SendLocal(new TestMessage());
            Console.ReadKey();

        }
    }

    public class TestMessage : ICommand
    {
    }

    public class TestMessageHandler : IHandleMessages<TestMessage>
    {
        private ServiceA serviceA;
        private ServiceB serviceB;

        public TestMessageHandler(ServiceB serviceB, ServiceA serviceA)
        {
            this.serviceB = serviceB;
            this.serviceA = serviceA;
        }

        public Task Handle(TestMessage message, IMessageHandlerContext context)
        {
            Console.WriteLine("message handled");
            return Task.FromResult(0);
        }
    }

    public class ServiceA
    {

    }

    public class ServiceB
    {

    }
}