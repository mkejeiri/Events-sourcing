using System;
using NServiceBus;

namespace eCommerce.Messages
{
    //The process start by sending this command to Saga
    //The sender could be the MVC client or RestApi
    public class ProcessOrderCommand: ICommand
    {
        public Guid OrderId { get; set; }
        public string AddressFrom { get; set; }
        public string AddressTo { get; set; }
        public int Weight { get; set; }
        public int Price { get; set; }
    }
}
