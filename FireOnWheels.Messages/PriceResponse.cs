using NServiceBus;

namespace FireOnWheels.Messages
{
    public class PriceResponse: IMessage
    {
        public int Price { get; set; }
    }
}
