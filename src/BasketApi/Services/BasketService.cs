﻿using BasketApi.Exceptions;
using BasketApi.Models;
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
            await CollectBasketProducts(id, basket);
            await CollectBasketDiscounts(basket);

            var basketState = basket.ToBasketState();
            return basketState;
        }

        private async Task CollectBasketProducts(string id, Basket basket)
        {
            IEnumerable<BasketEvent> basketEvents;
            try
            {
                basketEvents = await _basketRepository.GetBasketEvents(id);
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                throw new BasketRequestException("No basket found with id " + id);
            }

            foreach (var basketEvent in basketEvents)
            {
                var addToBasketEvent = (AddToBasketEvent)basketEvent;
                var price = await _productService.GetPrice(addToBasketEvent.SKU);
                basket.AddProduct(addToBasketEvent.SKU, addToBasketEvent.Quantity, price);

            }
        }
            private async Task CollectBasketDiscounts(Basket basket)
        {
            foreach (var item in basket.Products)
            {
                var rule = await _discountService.GetDiscount(item.SKU);
                if (rule != null)
                {
                    basket.AddDiscount(rule);
                }
            }
        }

        public async Task AddToBasket(string id, string? sku, int? quantity)
        {
            if (!await _productService.ProductExists(sku))
            {
                throw new BasketRequestException("No product matching the sku \"" + sku + "\" exists");
            }
            await _basketRepository.AddBasketEvents(id, new AddToBasketEvent(sku, (int)quantity));
        }
    }
}
