namespace BasketApi.Storage;

//
// This is very much a fake implementation to avoid the pain of running a database.
// Everything is just held in memory which of course means everything is lost on restart.
//

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

public class DiscountsRepository
{
    private readonly ILogger<DiscountsRepository> _logger;
    private readonly string _connectionString;

    private static List<DiscountRule?> _discountRules = new()
    {
        DiscountRule.PercentageRule("sku1", "12% of Alpha's", 12),
        DiscountRule.BOGOFRule("sku2", "Buy 3 Betas get 1 free", 3),
    };

    public DiscountsRepository(ILogger<DiscountsRepository> logger, string connectionString)
    {
        _logger = logger;
        _connectionString = connectionString;
    }

    public DiscountsRepository()
    {
    }

    public async Task<IEnumerable<DiscountRule?>> GetDiscountRules()
    {
        return _discountRules;
    }
}

public enum DiscountType
{
    Percentage,
    Bogof,
}

public class DiscountRule
{
    public DiscountRule(DiscountType type, string description, string productSKU, float? percentage = null, int? minimumRequired = null)
    {
        Type = type;
        Description = description;
        ProductSKU = productSKU;
        Percentage = percentage;
        MinimumRequired = minimumRequired;
    }

    public static DiscountRule PercentageRule(string productSKU, string description, float percentage)
    {
        return new DiscountRule(DiscountType.Percentage, description, productSKU, percentage: percentage);
    }

    internal static DiscountRule BOGOFRule(string productSKU, string description, int minimumRequired)
    {
        return new DiscountRule(DiscountType.Bogof, description, productSKU, minimumRequired: minimumRequired);
    }

    public DiscountType Type { get; init; }
    public string Description { get; init; }
    public float? Percentage { get; init; }
    public string ProductSKU { get; init; }
    public int? MinimumRequired { get; init; }
}
