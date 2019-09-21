using System.Threading.Tasks;
using FireOnWheels.Messages;
using NServiceBus;

namespace FireOnWheels.Web.Handlers
{
    public class OrderProcessedEventHandler :
        IHandleMessages<IOrderProcessedEvent>
    {
        public async Task Handle(IOrderProcessedEvent message, IMessageHandlerContext context)
        {
        }
    }
}