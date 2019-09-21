using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireOnWheels.Messages
{
    public class DispatchOrderCommand
    {
        public string AddressTo { get; set; }
        public int Weight { get; set; }
    }
}
