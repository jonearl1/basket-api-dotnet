using BasketApi.Controllers;
using BasketApi.Models;
using BasketApi.Storage;

namespace BasketApi.Services
{
    public class ProductService
    {
        private readonly ProductsRepository _productsRepository;
        public ProductService(ProductsRepository productsRepository)
        {
            _productsRepository = productsRepository;
        }

        public async Task<bool> ProductExists(string sku)
        {
            try
            {
                var product = await _productsRepository.GetProduct(sku);
            }
            catch(System.Collections.Generic.KeyNotFoundException)
            {
                return false;
            }

            return true;
        }

        public async Task<decimal> GetPrice(string? sku)
        {
            var product = await _productsRepository.GetProduct(sku);
            return product.Price;
        }
    }
}
