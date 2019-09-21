using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;

namespace eCommerce.Messages
{
    public class PlanOrderCommand : IMessage
    {
        public Guid OrderId { get; set; }
        public string AddressTo { get; set; }
    }
}
