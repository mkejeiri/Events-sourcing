using System.Threading.Tasks;
using NServiceBus;
using ServiceControl.Contracts;

namespace eCommerce.Monitoring
{
    //Final step: MessageFailedHandler handles the MessageFailed event.
    //The type is present in the ServiceControl.Contracts assembly.
    //code to send a notification.
    public class MessageFailedHandler: IHandleMessages<MessageFailed>
    {
        public async Task Handle(MessageFailed message, IMessageHandlerContext context)
        {
            string failedMessageId = message.FailedMessageId;
            string exceptionMessage = message.FailureDetails.Exception.Message;

            //here code to send a notification
        }
    }
}
