using System;
using NServiceBus;

namespace FireOnWheels.Messages
{
    public class ProcessOrderCommand: ICommand
    {
        public Guid OrderId { get; set; }
        public string AddressFrom { get; set; }
        public string AddressTo { get; set; }
        public int Weight { get; set; }
        public int Price { get; set; }
    }
}
