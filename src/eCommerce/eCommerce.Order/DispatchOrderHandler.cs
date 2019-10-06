using System.Threading.Tasks;
using eCommerce.Messages;
using eCommerce.Order.Helper;
using NServiceBus;
using NServiceBus.Logging;

namespace eCommerce.Dispatch
{
    public class DispatchOrderHandler : IHandleMessages<DispatchOrderCommand>
    {
        private static readonly ILog Logger =
            LogManager.GetLogger(typeof(DispatchOrderHandler));

        public async Task Handle(DispatchOrderCommand message, IMessageHandlerContext context)
        {
            Logger.InfoFormat("Order received! To address: {0}", message.AddressTo);
            await EmailSender.SendEmailToDispatch(message);

            await context.Reply<IOrderDispatchedMessage>(e => { }).ConfigureAwait(false);
        }
    }
}