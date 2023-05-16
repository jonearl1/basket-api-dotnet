using BasketApi.Controllers;
using BasketApi.Storage;

namespace BasketApi.Models;

public class Basket
{
    public string? Id { get; set; }
    public List<Product?> Products { get; set; } = new();

    private HashSet<DiscountRule> _discountRules = new();


    public void AddProduct(string? sku, int quantity, decimal price)
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

    public void AddDiscount(DiscountRule discountRule)
    {
        _discountRules.Add(discountRule);
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
        {
            basketState.SubTotal = basketState.Items.Sum(item => item.Total);

            foreach (var discount in _discountRules)
            {
                var item = basketState.Items.FirstOrDefault(item => item.SKU == discount.ProductSKU);
                if (item != null)
                {
                    if (discount.Type == DiscountType.Percentage)
                    {
                        item.DiscountDescription = discount.Description;
                        item.Discount = item.Total * (decimal?)discount.Percentage * 0.01m;
                    }

                    if (discount.Type == DiscountType.Bogof && item.Quantity >= discount.MinimumRequired)
                    {
                        item.DiscountDescription = discount.Description;
                        item.Discount = (item.Quantity / discount.MinimumRequired) * item.Price;
                    }
                }
            }

            basketState.Discount = basketState.Items.Sum(item => item.Discount);
        }

        basketState.DiscountedTotal = basketState.SubTotal - basketState.Discount;

        return basketState;
    }
}