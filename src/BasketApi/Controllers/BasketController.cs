using Microsoft.AspNetCore.Mvc;

namespace BasketApi.Controllers;

[ApiController]
[Route("[controller]")]
public class BasketController : ControllerBase
{
    private readonly ILogger<BasketController> _logger;

    public BasketController(ILogger<BasketController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<BasketState> Get()
    {
        return new BasketState();
    }

    [HttpPost("{id}/add")]
    public async Task<BasketState> AddToBasket(string id, AddToBasketRequest request)
    {
        // service add item with i
        // service get basket state
        return new BasketState();
    }
}

public class AddToBasketRequest
{
    public string? SKU { get; set; }
    public int? Quantity { get; set; }
}