using BasketApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using BasketApi.Models;

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
    public async Task<BasketState> Get(string id)
    {
        return await _basketService.GetBasket(id);
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
    [Required]
    public string? SKU { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a number greater than 0")]
    public int? Quantity { get; set; }
}

