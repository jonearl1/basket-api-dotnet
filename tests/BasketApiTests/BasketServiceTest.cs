using BasketApi.Exceptions;
using BasketApi.Services;
using BasketApi.Storage;
using System.Threading.Tasks;
using Xunit;

namespace BasketApiTests
{
    public class BasketServiceTest
    {
        private readonly BasketService _basketService;

        public BasketServiceTest()
        {
            var basketRepository = new BasketRepository();
            basketRepository.ResetBasketEvents();
            _basketService = new BasketService(basketRepository, new ProductService(new ProductsRepository()), new DiscountService(new DiscountsRepository()));
        }

        [Fact]
        public async Task AddOneItem()
        {
            const string basketId = "1";
            const string? sku = "sku1";
            const int quantity = 1;

            await _basketService.AddToBasket(basketId, sku, quantity);
            var basketState = await _basketService.GetBasket(basketId);

            Assert.Equal(basketId, basketState.Id);
            var item = basketState.Items?[0]!;
            Assert.Equal(sku, item.SKU);
            Assert.Equal(quantity, item.Quantity);
            Assert.Equal(12.30m, item.Total);
        }

        [Fact]
        public async Task AddSecondItem()
        {
            const string basketId = "1";
            const string? sku = "sku1";
            const int quantity = 1;

            const string? sku2 = "sku2";

            await _basketService.AddToBasket(basketId, sku, quantity);
            await _basketService.AddToBasket(basketId, sku2, quantity);

            var basketState = await _basketService.GetBasket(basketId);

            Assert.Equal(basketId, basketState.Id);
            var item = basketState.Items?[0]!;
            Assert.Equal(sku, item.SKU);
            Assert.Equal(quantity, item.Quantity);
            Assert.Equal(12.30m, item.Total);

            var item1 = basketState.Items?[1]!;
            Assert.Equal(sku2, item1.SKU);
            Assert.Equal(quantity, item1.Quantity);
            Assert.Equal(100.99m, item1.Total);
        }

        [Fact]
        public async Task AddSameItem()
        {
            const string basketId = "1";
            const string? sku = "sku1";
            const int quantity = 1;

            await _basketService.AddToBasket(basketId, sku, quantity);
            await _basketService.AddToBasket(basketId, sku, quantity);

            var basketState = await _basketService.GetBasket(basketId);

            var item = basketState.Items?[0]!;
            Assert.Equal(sku, item.SKU);
            Assert.Equal(quantity * 2, item.Quantity);
            Assert.Equal(24.60m, item.Total);
        }

        [Fact]
        public async Task AddSecondId()
        {
            const string basketId = "1";
            const string? sku = "sku1";
            const int quantity = 1;
            const string basketId2 = "2";

            await _basketService.AddToBasket(basketId, sku, quantity);
            await _basketService.AddToBasket(basketId2, sku, quantity);

            var basketState = await _basketService.GetBasket(basketId2);

            Assert.Equal(basketId2, basketState.Id);
            Assert.Equal(sku, basketState.Items?[0].SKU);
            Assert.Equal(quantity, basketState.Items?[0].Quantity);
        }

        [Fact]
        public async Task CalculateTotalOneItem()
        {
            await _basketService.AddToBasket("1", "sku1", 1);
            var basketState = await _basketService.GetBasket("1");

            Assert.Equal(12.30m, basketState.SubTotal);
            Assert.Equal(12.30m, basketState.Items?[0].Total);
        }

        [Fact]
        public async Task CalculateTotalMultipleItems()
        {
            await _basketService.AddToBasket("1", "sku1", 2);
            await _basketService.AddToBasket("1", "sku2", 3);
            await _basketService.AddToBasket("1", "sku3", 1);
            var basketState = await _basketService.GetBasket("1");

            Assert.Equal(378.56m, basketState.SubTotal);
            Assert.Equal(24.60m, basketState.Items?[0].Total);
            Assert.Equal(302.97m, basketState.Items?[1].Total);
            Assert.Equal(50.99m, basketState.Items?[2].Total);
        }


        [Fact]
        public async Task CalculatePercentageDiscountNoDiscount()
        {
            await _basketService.AddToBasket("1", "sku3", 1);
            var basketState = await _basketService.GetBasket("1");

            Assert.Equal(0m, basketState.Discount);
            Assert.Equal(50.99m, basketState.DiscountedTotal);
            Assert.Equal(0m, basketState.Items?[0].Discount);
            Assert.Null(basketState.Items?[0].DiscountDescription);
        }

        [Fact]
        public async Task CalculatePercentageDiscountOneItem()
        {
            await _basketService.AddToBasket("1", "sku1", 1);
            var basketState = await _basketService.GetBasket("1");

            Assert.Equal(1.476m, basketState.Discount);
            Assert.Equal(10.8240m, basketState.DiscountedTotal);
            Assert.Equal(1.476m, basketState.Items?[0].Discount);
            Assert.Equal("12% of Alpha's", basketState.Items?[0].DiscountDescription);
        }

        [Fact]
        public async Task CalculatePercentageDiscountManyItem()
        {
            await _basketService.AddToBasket("1", "sku1", 2);
            await _basketService.AddToBasket("1", "sku3", 3);
            var basketState = await _basketService.GetBasket("1");

            Assert.Equal(2.9520m, basketState.Discount);
            Assert.Equal(174.6180m, basketState.DiscountedTotal);
            Assert.Equal(2.9520m, basketState.Items?[0].Discount);
            Assert.Equal(0m, basketState.Items?[1].Discount);
        }

        [Fact]
        public async Task CalculateBogofDiscount3Items()
        {
            await _basketService.AddToBasket("1", "sku2", 3);
            var basketState = await _basketService.GetBasket("1");

            Assert.Equal(302.97m, basketState.SubTotal);
            Assert.Equal(100.99m, basketState.Discount);
            Assert.Equal(201.98m, basketState.DiscountedTotal);

            Assert.Equal(100.99m, basketState.Items?[0].Discount);
            Assert.Equal("Buy 3 Betas get 1 free", basketState.Items?[0].DiscountDescription);
        }

        [Fact]
        public async Task CalculateBogofDiscount8Items()
        {
            await _basketService.AddToBasket("1", "sku2", 8);
            var basketState = await _basketService.GetBasket("1");

            Assert.Equal(807.92m, basketState.SubTotal);
            Assert.Equal(201.98m, basketState.Discount);
            Assert.Equal(605.94m, basketState.DiscountedTotal);

            Assert.Equal(201.98m, basketState.Items?[0].Discount);
            Assert.Equal("Buy 3 Betas get 1 free", basketState.Items?[0].DiscountDescription);
        }

        [Fact]
        public async Task NoBogoffDiscountUnderMinAmount()
        {
            await _basketService.AddToBasket("1", "sku2", 2);
            var basketState = await _basketService.GetBasket("1");

            Assert.Equal(201.98m, basketState.SubTotal);
            Assert.Equal(0m, basketState.Discount);
            Assert.Equal(201.98m, basketState.DiscountedTotal);

            Assert.Equal(0m, basketState.Items?[0].Discount);
            Assert.Null(basketState.Items?[0].DiscountDescription);
        }

        [Fact]
        public async Task AddInvalidSku()
        {
            await Assert.ThrowsAsync<BasketRequestException>(() =>
                _basketService.AddToBasket("basketId", "not_on_the_list", 1));
        }

        [Fact]
        public async Task RemoveInvalidSku()
        {
            await Assert.ThrowsAsync<BasketRequestException>(() =>
                _basketService.RemoveFromBasket("basketId", "not_on_the_list"));
        }

        [Fact]
        public async Task RemoveOneItem()
        {
            const string basketId = "1";
            const string? sku = "sku3";

            await _basketService.AddToBasket(basketId, sku, 2);
            await _basketService.RemoveFromBasket(basketId, sku);


            var basketState = await _basketService.GetBasket(basketId);

            Assert.Equal(basketId, basketState.Id);
            Assert.Equal(50.99m, basketState.SubTotal);
            Assert.Equal(50.99m, basketState.Items?[0].Total);

            var item = basketState.Items?[0]!;
            Assert.Equal(sku, item.SKU);
            Assert.Equal(1, item.Quantity);
            Assert.Equal(50.99m, item.Total);
        }

        [Fact]
        public async Task RemoveOneNoItemsInBasket()
        {
            const string basketId = "1";
            const string? sku = "sku3";

            await Assert.ThrowsAsync<BasketRequestException>(() =>
                _basketService.RemoveFromBasket(basketId, sku));
        }
    }
}