namespace Storage;

//
// This is very much a fake implementation to avoid the pain of running a database.
// Everything is just held in memory which of course means everything is lost on restart.
//

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

public class ProductsRepository
{
    private readonly ILogger<ProductsRepository> _logger;
    private readonly string _connectionString;

    private static Dictionary<string, Product> _products = new()
    {
        {"sku1", new Product("sku1", "Alpha", 12.30m) },
        {"sku2", new Product("sku2", "Beta", 100.99m) },
        {"sku3", new Product("sku3", "Gamma", 50.99m) },
        {"skubag", new Product("skubag", "Bag", 50.99m) },
    };

    public ProductsRepository(ILogger<ProductsRepository> logger, string connectionString)
    {
        _logger = logger;
        _connectionString = connectionString;
    }

    public async Task<Product> GetProduct(string sku)
    {
        return _products[sku];
    }
}

public class Product
{
    public Product(string sku, string name, decimal price)
    {
        SKU = sku;
        Name = name;
        Price = price;
    }

    public string SKU { get; init; }
    public string Name { get; init; }
    public decimal Price { get; init; }
}
