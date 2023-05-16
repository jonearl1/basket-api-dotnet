using BasketApi.Controllers;
using BasketApi.Storage;

namespace BasketApi.Models;

public class Product
{
    public Product(string? sku, int quantity, decimal price)
    {
        SKU = sku;
        Quantity = quantity;
        Price = price;
    }

    public string? SKU { get; set; }
    public int? Quantity { get; set; }
    public decimal? Price { get; set; }

    public BasketItem ToItem()
    {
        BasketItem item = new()
        {
            SKU = SKU,
            Quantity = Quantity,
            Price = Price
        };
        item.Total = item.Quantity * item.Price;
        return item;
    }
}