using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using NServiceBus;

namespace FireOnWheels.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ConfigureEndpoint();
        }

        /*
      Although NServiceBus uses its own DI container internally, or any other DI to set up injection into the web API controls, 
      because that's not supported by NServiceBus container. The ContainerBuilder class for Autofac is called ContainerBuilder. 
      We registered the controllers with the container => thus the endpoint is registered. We build the container to tell Web API
      to use it by setting the DependencyResolver on its Configuration object. But from now on, We can inject the endpoint
      instance into the controller to do operations with messages.

      install-package Microsoft.AspNet.WebApi.OData -ProjectName 
      update-package Microsoft.AspNet.WebApi -ProjectName FireOnWheels.Rest
      */
        private void ConfigureEndpoint()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            var endpointConfiguration = new EndpointConfiguration("FireOnWheelsUI");
            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            endpointConfiguration.UseTransport<MsmqTransport>();
            endpointConfiguration.PurgeOnStartup(true);
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.MakeInstanceUniquelyAddressable("uniqueId");
            endpointConfiguration.UseContainer<AutofacBuilder>(
                customizations => {
                    customizations.ExistingLifetimeScope(container);
                });
            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            endpointConfiguration.EnableInstallers();

            var endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

            var updater = new ContainerBuilder();
            updater.RegisterInstance(endpoint);
            updater.RegisterControllers(typeof(MvcApplication).Assembly);
            var updated = updater.Build();

            //We can inject the endpoint instance into the controller to do operations with messages.
            DependencyResolver.SetResolver(new AutofacDependencyResolver(updated));
        }
    }
}