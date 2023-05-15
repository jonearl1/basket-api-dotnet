using BasketApi.Controllers;
using BasketApi.Exceptions;
using BasketApi.Storage;

namespace BasketApi.Services
{
    public class BasketService
    {
        private readonly BasketRepository _basketRepository;
        private readonly ProductService _productService;
        public BasketService(BasketRepository basketRepository, ProductService productService)
        {
            _basketRepository = basketRepository;
            _productService = productService;
        }

        public async Task<BasketState> GetBasket(string id)
        {
            BasketState basketState = new()
            {
                Id = id
            };
            foreach (var basketEvent in await _basketRepository.GetBasketEvents(id))
            {
                var addToBasketEvent = (AddToBasketEvent)basketEvent;
                var price = await _productService.GetPrice(addToBasketEvent.SKU);
                basketState.AddItem(addToBasketEvent.SKU, addToBasketEvent.Quantity, price);
            }
            return basketState;
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
