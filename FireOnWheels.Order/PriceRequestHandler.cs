﻿using System.Threading.Tasks;
using FireOnWheels.Order.Helper;
using FireOnWheels.Messages;
using NServiceBus;

namespace FireOnWheels.Order
{
    public class PriceRequestHandler: IHandleMessages<PriceRequest>
    {
        public async Task Handle(PriceRequest message, IMessageHandlerContext context)
        {
            //we don't have to configure routing for the message Reply.
            //the message contains the endpoint name of the sender
            //and NServiceBus will use this info to sendback the message
            await context.Reply(new PriceResponse {Price = await PriceCalculator.GetPrice(message)})
                .ConfigureAwait(false);
        }
    }
}
