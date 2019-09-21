using NServiceBus;

namespace eCommerce.Messages
{
    public class PriceResponse: IMessage
    {
        public int Price { get; set; }
    }
}
