using BasketApi.Exceptions;
using BasketApi.Services;
using BasketApi.Storage;
using System.Threading.Tasks;
using Xunit;

namespace BasketApiTests
{

    public class BasketServiceTest
    {
        private readonly BasketService basketService;

        public BasketServiceTest()
        {
            basketService = new BasketService(new BasketRepository(), new ProductService(new ProductsRepository()));
        }

        [Fact]
        public async Task AddOneItem()
        {
            const string basketId = "1";
            const string? sku = "sku1";
            const int quantity = 1;

            await basketService.AddToBasket(basketId, sku, quantity);
            var basketState = await basketService.GetBasket(basketId);

            Assert.Equal(basketId, basketState.Id);
            Assert.Equal(sku, basketState.Items?[0].SKU);
            Assert.Equal(quantity, basketState.Items?[0].Quantity);
        }

        [Fact]
        public async Task AddSecondItem()
        {
            const string basketId = "1";
            const string? sku = "sku1";
            const int quantity = 1;

            const string? sku2 = "sku2";

            await basketService.AddToBasket(basketId, sku, quantity);
            await basketService.AddToBasket(basketId, sku2, quantity);

            var basketState = await basketService.GetBasket(basketId);

            Assert.Equal(basketId, basketState.Id);
            Assert.Equal(sku, basketState.Items?[0].SKU);
            Assert.Equal(quantity, basketState.Items?[0].Quantity);
            Assert.Equal(sku2, basketState.Items?[1].SKU);
            Assert.Equal(quantity, basketState.Items?[1].Quantity);
        }

        [Fact]
        public async Task AddSameItem()
        {
            const string basketId = "1";
            const string? sku = "sku1";
            const int quantity = 1;

            await basketService.AddToBasket(basketId, sku, quantity);
            await basketService.AddToBasket(basketId, sku, quantity);

            var basketState = await basketService.GetBasket(basketId);

            Assert.Equal(basketId, basketState.Id);
            Assert.Equal(sku, basketState.Items?[0].SKU);
            Assert.Equal(quantity * 2, basketState.Items?[0].Quantity);
        }

        [Fact]
        public async Task AddSecondId()
        {
            const string basketId = "1";
            const string? sku = "sku1";
            const int quantity = 1;
            const string basketId2 = "2";

            await basketService.AddToBasket(basketId, sku, quantity);
            await basketService.AddToBasket(basketId2, sku, quantity);

            var basketState = await basketService.GetBasket(basketId2);

            Assert.Equal(basketId2, basketState.Id);
            Assert.Equal(sku, basketState.Items?[0].SKU);
            Assert.Equal(quantity, basketState.Items?[0].Quantity);
        }

        [Fact]
        public async Task NullSku()
        {
            await Assert.ThrowsAsync<BasketRequestException>(() => basketService.AddToBasket("basketId", null, 1));
        }

        [Fact]
        public async Task NullQuantity()
        {
            await Assert.ThrowsAsync<BasketRequestException>(() => basketService.AddToBasket("basketId", "sku1", null));
        }

        [Fact]
        public async Task InvalidSku()
        {
            await Assert.ThrowsAsync<BasketRequestException>(() =>
                basketService.AddToBasket("basketId", "not_on_the_list", 1));
        }
    }
}