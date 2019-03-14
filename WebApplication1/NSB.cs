using System;
using System.Threading.Tasks;
using NServiceBus;

namespace WebApplication1
{
    public class TestMessage : ICommand
    {
    }

    public class TestMessageHandler : IHandleMessages<TestMessage>
    {
        private ServiceA serviceA;
        private ServiceB serviceB;
        private ServiceC serviceC;

        public TestMessageHandler(ServiceB serviceB, ServiceA serviceA, ServiceC serviceC)
        {
            this.serviceB = serviceB;
            this.serviceA = serviceA;
            this.serviceC = serviceC;
        }

        public Task Handle(TestMessage message, IMessageHandlerContext context)
        {
            Console.WriteLine("message handled");
            return Task.FromResult(0);
        }
    }

    public class ServiceA
    {
        public ServiceA(string x)
        {

        }
    }

    public class ServiceB
    {
        public ServiceB(string x)
        {
        }
    }

    public class ServiceC
    {
        //public ServiceC(string x)
        //{
        //}
    }
}