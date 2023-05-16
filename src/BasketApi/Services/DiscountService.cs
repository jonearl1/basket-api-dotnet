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

        public async Task<DiscountRule?> GetPercentageDiscount(string? sku)
        {
            DiscountRule? discountRule = null;
            var discountRules = await _discountsRepository.GetDiscountRules();
            foreach (var rule in discountRules)
            {
                if (rule.Type == DiscountType.Percentage && rule.ProductSKU == sku )
                {
                    discountRule = rule;
                }
            }

            return discountRule;
        }
    }
}
