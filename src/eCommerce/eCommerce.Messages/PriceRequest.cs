using NServiceBus;

namespace eCommerce.Messages
{
    public class PriceRequest: IMessage
    {
        public int Weight { get; set; }
    }
}
