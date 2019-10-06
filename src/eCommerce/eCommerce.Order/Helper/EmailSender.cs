using System.Threading.Tasks;
using eCommerce.Messages;

namespace eCommerce.Order.Helper
{
    public static class EmailSender
    {
        public static async Task SendEmailToDispatch(ProcessOrderCommand order)
        {
            await Task.CompletedTask;
        }

        public static async Task SendEmailToDispatch(DispatchOrderCommand order)
        {
            await Task.CompletedTask;
        }
    }
}
