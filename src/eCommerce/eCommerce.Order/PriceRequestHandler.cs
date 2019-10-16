using System.Threading.Tasks;
using eCommerce.Order.Helper;
using eCommerce.Messages;
using NServiceBus;

namespace eCommerce.Order
{
    public class PriceRequestHandler: IHandleMessages<PriceRequest>
    {
        public async Task Handle(PriceRequest message, IMessageHandlerContext context)
        {
            //we don't have to configure routing for the message Reply.
            //the message contains the endpoint name of the sender
            //and NServiceBus will use this info to sendback the message
            await context.Reply(new PriceResponse {Price = await PriceCalculator.GetPrice(message)})
                // prevent the passing in of the controls thread context into the new
                // thread, which we don't need for sending a message
                .ConfigureAwait(false);
        }
    }
}
