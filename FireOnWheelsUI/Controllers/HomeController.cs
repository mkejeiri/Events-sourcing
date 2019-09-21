using System.Threading.Tasks;
using System.Web.Mvc;
using FireOnWheels.Messages;
using NServiceBus;
using Order = FireOnWheels.Web.Models.Order;

namespace FireOnWheels.Web.Controllers
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
            var options = new SendOptions();
            options.SetDestination("FireOnWheels.Order");

            //Call Request on the endpoint where we specify PriceResponse as response,
            //we have to set up a RoutingTo for PriceRequest in the web.config (UnicastBusConfig section).
            //For the Request method to work, we need install also the NServiceBus Callbacks NuGet package.
            //In addition to that, we have to configure a uniqueId in the endpoint configuration for the service. 
            //Request is an extension method in the NServiceBus.Callbacks NuGet package
            var priceResponse = await endpoint.Request<PriceResponse>(new PriceRequest {Weight = order.Weight}, options);
            order.Price = priceResponse.Price;
            return View("Review", order);
        }

        //endpoint is asynchronous, actual work involved e.g. sending the message, doesn't block the thread where the controllers run on
        //while the message is sent, controllers are able to process other requests
        public async Task<ActionResult> Confirm(Order order)
        {
            await endpoint.Send("FireOnWheels.Order", new ProcessOrderCommand
            {
                AddressFrom = order.AddressFrom,
                AddressTo = order.AddressTo,
                Price = order.Price,
                Weight = order.Weight
            }).ConfigureAwait(false);

            return View();
        }
    }
}
