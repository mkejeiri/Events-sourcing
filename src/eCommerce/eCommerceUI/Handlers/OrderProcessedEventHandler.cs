using System.Threading.Tasks;
using eCommerce.Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace eCommerce.Web.Handlers
{
    public class OrderProcessedEventHandler :
        IHandleMessages<IOrderProcessedEvent>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OrderProcessedEventHandler));
        public async Task Handle(IOrderProcessedEvent message, IMessageHandlerContext context)
        {
            //notify the user with signalR
            Logger.Info($"OrderProcessed event received! Price: {message.Price} , Weight : {message.Weight}");
            await  Task.CompletedTask;
        }
    }
}