namespace BasketApi.Models;

public class BasketItem
{
    public BasketItem(string? sku, int quantity, decimal price)
    {
        SKU = sku;
        Quantity = quantity;
        Price = price;
        UpdateTotal();
    }

    public string? SKU { get; set; }
    public int? Quantity { get; set; }
    public decimal? Price { get; set; }
    public decimal? Total { get; set; }
    public decimal? Discount { get; set; }
    public string? DiscountDescription { get; set; }

    public void UpdateTotal()
    {
        Total = Quantity * Price;
    }
}