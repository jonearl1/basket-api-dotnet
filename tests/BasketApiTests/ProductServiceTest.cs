using BasketApi.Services;
using BasketApi.Storage;
using System.Threading.Tasks;
using Xunit;

namespace BasketApiTests;

public class ProductServiceTest
{

    [Fact]
    public async Task ItemExists()
    {
        var productService = new ProductService(new ProductsRepository());
        const string sku = "sku1";

        Assert.True(await productService.ProductExists(sku));
    }

    [Fact]
    public async Task ItemDoesNotExist()
    {
        var productService = new ProductService(new ProductsRepository());
        const string sku = "invalid";

        Assert.False(await productService.ProductExists(sku));
    }
}