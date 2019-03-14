using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NServiceBus;

namespace WebApplication1.Controllers
{
    public class CommandController : Controller
    {
        private IMessageSession session;
        private ServiceA serviceA;
        private ServiceB serviceB;
        private ServiceC serviceC;

        public CommandController(IMessageSession session, ServiceA serviceA, ServiceB serviceB, ServiceC serviceC)
        {
            this.session = session;
            this.serviceA = serviceA;
            this.serviceB = serviceB;
            this.serviceC = serviceC;
        }

        public IActionResult Index()
        {
            return View("Privacy");
        }

        [HttpPost]
        public async Task<IActionResult> Send()
        {
            await session.SendLocal(new TestMessage());
            return View("Index");
        }
    }
}