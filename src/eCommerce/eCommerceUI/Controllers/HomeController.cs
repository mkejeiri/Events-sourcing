using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using eCommerce.Messages;
using NServiceBus;
using Order = eCommerce.Web.Models.Order;

namespace eCommerce.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEndpointInstance endpoint;

        public HomeController(IEndpointInstance endpoint)
        {
            this.endpoint = endpoint;
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(Order order)
        {
            //Routing is done in the config file
            //var options = new SendOptions();
            //options.SetDestination("eCommerce.Order");

            //Call Request on the endpoint where we specify PriceResponse as response,
            //we have to set up a RoutingTo for PriceRequest in the web.config (UnicastBusConfig section).
            //For the Request method to work, we need install also the NServiceBus Callbacks NuGet package.
            //In addition to that, we have to configure a uniqueId in the endpoint configuration for the service. 
            //Request is an extension method in the NServiceBus.Callbacks NuGet package
            var priceResponse = await endpoint.Request<PriceResponse>(
                new PriceRequest { Weight = order.Weight }
                //, options //Routing is done in the config file
                );
            order.Price = priceResponse.Price;

            //Request is an extension method in the NServiceBus.Callbacks NuGet package
            //You also need to assign a unique id to the endpoint
            return View("Review", order);
        }

        //endpoint is asynchronous, actual work involved e.g. sending the message, doesn't block the thread where the controllers run on
        //while the message is sent, controllers are able to process other requests
        public async Task<ActionResult> Confirm(Order order)
        {
            //await endpoint.Send("eCommerce.Order", new ProcessOrderCommand
            //Routing is done in the config file
            await endpoint.Send(message: new ProcessOrderCommand
            {
                OrderId = Guid.NewGuid(),
                AddressFrom = order.AddressFrom,
                AddressTo = order.AddressTo,
                Price = order.Price,
                Weight = order.Weight
            })
                // prevent the passing in of the controls thread context into the new
                // thread, which we don't need for sending a message
                .ConfigureAwait(continueOnCapturedContext: false);

            return View();
        }
    }
}
