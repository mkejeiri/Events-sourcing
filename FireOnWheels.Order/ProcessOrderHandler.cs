﻿using System.Threading.Tasks;
using FireOnWheels.Messages;
using FireOnWheels.Order.Helper;
using NServiceBus;
using NServiceBus.Logging;

namespace FireOnWheels.Order
{
    public class ProcessOrderHandler : IHandleMessages<ProcessOrderCommand>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ProcessOrderHandler));

        public async Task Handle(ProcessOrderCommand message, IMessageHandlerContext context)
        {
            Logger.InfoFormat("Order received! To address: {0}", message.AddressTo);
            await EmailSender.SendEmailToDispatch(message);

            await context.Publish<IOrderProcessedEvent>(e =>
            {
                e.AddressFrom = message.AddressFrom;
                e.AddressTo = message.AddressTo;
                e.Price = message.Price;
                e.Weight = message.Weight;
            });

            //await context.Reply<IOrderDispatchedMessage>(e => { }).ConfigureAwait(false);
        }
    }
}
