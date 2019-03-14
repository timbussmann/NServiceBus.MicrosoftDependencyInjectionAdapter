using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lamar;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NServiceBus;
using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.MicrosoftDependencyInjectionAdapter;
using Endpoint = Microsoft.AspNetCore.Http.Endpoint;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var ec = new EndpointConfiguration("DItestASP");

            //registry.For<ServiceA>().Use<ServiceA>().Singleton();
            services.AddSingleton<ServiceA>(_ => new ServiceA("a"));

            //services.AddSingleton<IServiceProviderFactory<IServiceCollection>>(hack);
            //services.AddSingleton<IServiceProviderFactory<ServiceRegistry>>(hack);
            ec.GetSettings().Set(typeof(IServiceCollection).FullName, services);
            IServiceProvider serviceProvider = null;
            ec.GetSettings().Set("MSDIAdapter.Factory", value: (Func<IServiceCollection, IServiceProvider>)(sc =>
            {
                sc.AddSingleton<ServiceC>();
                serviceProvider = new Container(sc);
                return serviceProvider;
            }));
            ec.UseContainer<MSDIAdapter>();

            ec.UseTransport<LearningTransport>();

            //ec.RegisterComponents(c => c.ConfigureComponent(typeof(ServiceA), DependencyLifecycle.SingleInstance));
            ec.RegisterComponents(c => c.RegisterSingleton(new ServiceB("b")));

            IEndpointInstance i = null;
            services.AddSingleton<IMessageSession>(_ => i);
            i = NServiceBus.Endpoint.Start(ec).GetAwaiter().GetResult();


            return serviceProvider;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
