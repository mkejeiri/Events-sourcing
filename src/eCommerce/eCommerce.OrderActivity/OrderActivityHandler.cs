using System.Threading.Tasks;
using eCommerce.Messages;
using NServiceBus;

namespace eCommerce.OrderActivity
{
    public class OrderActivityHandler: IHandleMessages<IOrderActivityEvent>
    {
        public async Task Handle(IOrderActivityEvent message,IMessageHandlerContext context)
        {
            //e.g. do your auditing here...
        }
    }
}
