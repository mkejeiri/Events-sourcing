using NServiceBus;

namespace eCommerce.Messages
{
    public interface IOrderProcessedEvent : IEvent
    {
        string AddressFrom { get; set; }
        string AddressTo { get; set; }
        int Weight { get; set; }
        int Price { get; set; }
    }
}
