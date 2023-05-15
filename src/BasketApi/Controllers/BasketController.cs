using BasketApi.Exceptions;
using BasketApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BasketApi.Controllers;

[ApiController]
[Route("[controller]")]
public class BasketController : ControllerBase
{
    private readonly ILogger<BasketController> _logger;
    private BasketService _basketService;

    public BasketController(ILogger<BasketController> logger, BasketService basketService)
    {
        _logger = logger;
        _basketService = basketService;
    }

    [HttpGet("{id}")]
    public async Task<BasketState> Get()
    {
        return new BasketState();
    }

    [HttpPost("{id}/add")]
    public async Task<BasketState> AddToBasket(string id, AddToBasketRequest request)
    {
        await _basketService.AddToBasket(id, request.SKU, request.Quantity);

        return await _basketService.GetBasket(id);
    }
}

public class AddToBasketRequest
{
    public string? SKU { get; set; }
    public int? Quantity { get; set; }
}