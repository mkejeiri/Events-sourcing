using System.Threading.Tasks;
using FireOnWheels.Messages;

namespace FireOnWheels.Order.Helper
{
    public static class PriceCalculator
    {
        public static async Task<int> GetPrice(PriceRequest priceRequest)
        {
            return await Task.FromResult(priceRequest.Weight < 10 ? 6 : 10);
        }
    }
}
