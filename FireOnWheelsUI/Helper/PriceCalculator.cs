using FireOnWheelsUI.Models;

namespace FireOnWheelsUI.Helper
{
    public static class PriceCalculator
    {
        public static int GetPrice(Order order)
        {
            return order.Weight < 10 ? 6 : 10;
        }
    }
}
