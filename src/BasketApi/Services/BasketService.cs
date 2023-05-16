using BasketApi.Controllers;
using BasketApi.Exceptions;
using BasketApi.Storage;
using System.Collections.Generic;

namespace BasketApi.Services
{
    public class BasketService
    {
        private readonly BasketRepository _basketRepository;
        private readonly ProductService _productService;
        private readonly DiscountService _discountService;
        public BasketService(BasketRepository basketRepository, ProductService productService, DiscountService discountService)
        {
            _basketRepository = basketRepository;
            _productService = productService;
            _discountService = discountService;
        }

        public async Task<BasketState> GetBasket(string id)
        {
            Basket basket = new()
            {
                Id = id
            };
            await AddProductsToBasket(id, basket);

            var basketState = basket.ToBasketState();
            // await AddDiscountsToBasket(id, basketState);
            return basketState;
        }

        // private async Task AddDiscountsToBasket(string id, BasketState basketState)
        // {
        //     HashSet<DiscountRule> ruleSet = new();
        //     foreach (var item in basketState.Products)
        //     {
        //         var rule = await _discountService.GetPercentageDiscount(item.SKU);
        //         if (rule != null)
        //         {
        //             ruleSet.Add(rule);
        //         }
        //     }
        //
        //     basketState.ApplyDiscounts(ruleSet);
        // }

        private async Task AddProductsToBasket(string id, Basket basket)
        {
            foreach (var basketEvent in await _basketRepository.GetBasketEvents(id))
            {
                var addToBasketEvent = (AddToBasketEvent)basketEvent;
                var price = await _productService.GetPrice(addToBasketEvent.SKU);
                basket.AddItem(addToBasketEvent.SKU, addToBasketEvent.Quantity, price);
            }
        }

        public async Task AddToBasket(string id, string? sku, int? quantity)
        {
            if (sku == null)
            {
                throw new BasketRequestException("sku must not be null");
            }
            if (quantity == null)
            {
                throw new BasketRequestException("quantity must not be null");
            }
            if (!await _productService.ProductExists(sku))
            {
                throw new BasketRequestException("No product matching the sku \"" + sku + "\" exists");
            }
            await _basketRepository.AddBasketEvents(id, new AddToBasketEvent(sku, (int)quantity));
        }
    }
}
