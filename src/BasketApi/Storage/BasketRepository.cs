namespace BasketApi.Storage;

//
// This is very much a fake implementation to avoid the pain of running a database.
// Everything is just held in memory which of course means everything is lost on restart.
//

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
public class BasketRepository
{
    private readonly ILogger<BasketRepository>? _logger;
    private readonly string? _connectionString;


    private static Dictionary<string, List<BasketEvent>> _basketEvents = new();


    public BasketRepository(ILogger<BasketRepository>? logger, string? connectionString)
    {
        _logger = logger;
        _connectionString = connectionString;
    }

    public BasketRepository()
    {
    }

    public async Task<IEnumerable<BasketEvent>> GetBasketEvents(string id)
    {
        return _basketEvents[id];
    }

    public async Task AddBasketEvents(string id, BasketEvent ev)
    {
        if (!_basketEvents.ContainsKey(id))
        {
            _basketEvents[id] = new List<BasketEvent>();
        }
        _basketEvents[id].Add(ev);
    }

    public void ResetBasketEvents()
    {
        _basketEvents = new Dictionary<string, List<BasketEvent>>();
    }
}

public abstract class BasketEvent
{
}

public class AddToBasketEvent : BasketEvent
{
    public AddToBasketEvent(string? sku, int quantity)
    {
        SKU = sku;
        Quantity = quantity;
    }
    public string? SKU { get; init; }
    public int Quantity { get; init; }
}