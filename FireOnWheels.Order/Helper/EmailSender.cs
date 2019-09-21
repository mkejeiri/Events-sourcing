using System.Threading.Tasks;
using FireOnWheels.Messages;

namespace FireOnWheels.Order.Helper
{
    public static class EmailSender
    {
        public static async Task SendEmailToDispatch(ProcessOrderCommand order)
        {
            await Task.CompletedTask;
        }
    }
}
