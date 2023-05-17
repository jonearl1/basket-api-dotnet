using BasketApi.Models;
using BasketApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BasketApi.Controllers;

[ApiController]
[Route("[controller]/{id}")]
public class BasketController : ControllerBase
{
    private readonly ILogger<BasketController> _logger;
    private BasketService _basketService;

    public BasketController(ILogger<BasketController> logger, BasketService basketService)
    {
        _logger = logger;
        _basketService = basketService;
    }

    [HttpGet("")]
    public async Task<BasketState> Get(string id)
    {
        return await _basketService.GetBasket(id);
    }

    [HttpPost("add")]
    public async Task<BasketState> AddToBasket(string id, AddToBasketRequest request)
    {
        await _basketService.AddToBasket(id, request.SKU, request.Quantity.Value);

        return await _basketService.GetBasket(id);
    }


    [HttpPost("remove")]
    public async Task<BasketState> RemoveFromBasket(string id, RemoveFromBasketRequest request)
    {
        await _basketService.RemoveFromBasket(id, request.SKU, request.Quantity.Value);

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
public class RemoveFromBasketRequest
{
    [Required]
    public string? SKU { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a number greater than 0")]
    public int? Quantity { get; set; }
}
