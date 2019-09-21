using System;
using System.Threading.Tasks;
using System.Web.Http;
using eCommerce.Messages;
using NServiceBus;
using Order = eCommerce.Rest.Models.Order;

namespace eCommerce.Rest.Controllers
{
    public class RestApiController : ApiController
    {
        private readonly IEndpointInstance endpoint;

        public RestApiController(IEndpointInstance endpoint)
        {
            this.endpoint = endpoint;
        }

        //endpoint is asynchronous, actual work involved e.g. sending the message, doesn't block the thread where the controllers run on
        //while the message is sent, controllers are able to process other requests
        public async Task Post(Order order)
        {
            await endpoint.Send("eCommerce.Order",new ProcessOrderCommand
            {
                OrderId = Guid.NewGuid(),
                AddressFrom = order.AddressFrom,
                AddressTo = order.AddressTo,
                Price = order.Price,
                Weight = order.Weight
            }).ConfigureAwait(false);
        }

    }
}