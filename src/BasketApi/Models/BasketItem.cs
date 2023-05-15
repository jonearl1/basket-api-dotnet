namespace BasketApi.Models;

public class BasketItem
{
    public BasketItem(string? sku, int quantity)
    {
        SKU = sku;
        Quantity = quantity;
    }

    public string? SKU { get; set; }
    public int? Quantity { get; set; }
    public decimal? Price { get; set; }
    public decimal? Total { get; set; }
    public decimal? Discount { get; set; }
    public string? DiscountDescription { get; set; }
}