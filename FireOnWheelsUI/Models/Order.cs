using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FireOnWheels.Web.Models
{
    public class Order
    {
        [DisplayName("From which address should we pick up the package?")]
        public string AddressFrom { get; set; }
        [DisplayName("To which address should we deliver the package?")]
        public string AddressTo { get; set; }
        [DisplayName("Weight of package")]
        public int Weight { get; set; }
        [ScaffoldColumn(false)]
        [DisplayName("The price of your package is")]
        public int Price { get; set; }
    }
}
