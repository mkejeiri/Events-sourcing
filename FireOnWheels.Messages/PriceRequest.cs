using NServiceBus;

namespace FireOnWheels.Messages
{
    public class PriceRequest: IMessage
    {
        public int Weight { get; set; }
    }
}
