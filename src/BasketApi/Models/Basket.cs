using BasketApi.Models;

namespace BasketApi.Controllers;

public class Basket
{
    public string? Id { get; set; }
    public List<Product?> Products { get; set; } = new();
    public decimal? SubTotal { get; set; }
    public decimal? Discount { get; set; }
    public string? DiscountedTotal { get; set; }

    public void AddItem(string? sku, int quantity, decimal price)
    {
        var product = Products.Find(i => i?.SKU == sku);
        if (product != null)
        {
            product.Quantity += quantity;
        }
        else
        {
            Products?.Add(new Product(sku, quantity, price));
        }
        
    }

    public BasketState ToBasketState()
    {
        BasketState basketState = new()
        {
            Id = Id
        };
        foreach (var item in Products.Select(product => product?.ToItem()))
        {
            if (item != null)
            {
                basketState.Items ??= new List<BasketItem>();
                basketState.Items?.Add(item);
            }
        }

        if (basketState.Items != null)
            basketState.SubTotal = basketState.Items.Sum(item => item.Total);

        return basketState;
    }
}