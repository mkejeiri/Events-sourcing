using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using NServiceBus;
using Autofac;
using Autofac.Integration.WebApi;

namespace FireOnWheels.Rest
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            var x = Assembly.GetExecutingAssembly();
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
            var endpointConfiguration = new EndpointConfiguration("FireOnWheels.Rest");
            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            endpointConfiguration.UseTransport<MsmqTransport>();
            endpointConfiguration.PurgeOnStartup(true);
            endpointConfiguration.EnableInstallers();


            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            endpointConfiguration.EnableInstallers();

            var endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            containerBuilder.RegisterInstance(endpoint);

            var container = containerBuilder.Build();

            endpointConfiguration.UseContainer<AutofacBuilder>(
                customizations =>
                {
                    customizations.ExistingLifetimeScope(container);

                });

            //We can inject the endpoint instance into the controller to do operations with messages.
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}