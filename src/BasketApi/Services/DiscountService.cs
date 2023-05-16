using BasketApi.Controllers;
using BasketApi.Models;
using BasketApi.Storage;

namespace BasketApi.Services
{
    public class DiscountService
    {
        private readonly DiscountsRepository _discountsRepository;
        public DiscountService(DiscountsRepository discountsRepository)
        {
            _discountsRepository = discountsRepository;
        }

        public async Task<DiscountRule?> GetDiscount(string? sku)
        {
            var discountRules = await _discountsRepository.GetDiscountRules();
            return discountRules.FirstOrDefault(rule => rule?.ProductSKU == sku);
        }
    }
}
