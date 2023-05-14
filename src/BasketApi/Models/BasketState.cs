using BasketApi.Models;

namespace BasketApi.Controllers;

public class BasketState
{
    public string? Id { get; set; }
    public List<BasketItem>? Items { get; set; } = new();
    public decimal? SubTotal { get; set; }
    public decimal? Discount { get; set; }
    public string? DiscountedTotal { get; set; }

    public void AddItem(string sku, int quantity)
    {
        Items?.Add(new BasketItem(sku, quantity));
    }
}