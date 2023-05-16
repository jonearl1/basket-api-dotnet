namespace BasketApi.Models;

public class BasketState
{

    public string? Id { get; set; }
    public List<BasketItem>? Items { get; set; }
    public decimal? SubTotal { get; set; }
    public decimal? Discount { get; set; }
    public decimal? DiscountedTotal { get; set; }
}