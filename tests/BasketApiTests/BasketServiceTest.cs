using System.Collections;
using System.Collections.Generic;
using BasketApi.Controllers;
using BasketApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BasketApiTests;
using BasketApi.Services;

public class BasketServiceTest
{
    [Fact]
    public void InitialBasket()
    {
        var basketService = new BasketService();
        var expectedBasket = new BasketState();

        var basketState = basketService.GetBasket("1");
        Assert.Equivalent(expectedBasket, basketState);
    }

    [Fact]
    public void AddOneItem()
    {
        var basketService = new BasketService();
        const string basketId = "1";
        const string sku = "sku1";
        const int quantity = 1;

        basketService.AddToBasket(basketId, sku, quantity);
        var basketState = basketService.GetBasket(basketId);

        Assert.Equal(basketId, basketState.Id);
        Assert.Equal(sku, basketState.Items?[0].SKU);
        Assert.Equal(quantity, basketState.Items?[0].Quantity);
    }

    [Fact]
    public void AddSecondItem()
    {
        var basketService = new BasketService();
        const string basketId = "1";
        const string sku = "sku1";
        const int quantity = 1;

        const string sku2 = "sku2";

        basketService.AddToBasket(basketId, sku, quantity);
        basketService.AddToBasket(basketId, sku2, quantity);

        var basketState = basketService.GetBasket(basketId);

        Assert.Equal(basketId, basketState.Id);
        Assert.Equal(sku, basketState.Items?[0].SKU);
        Assert.Equal(quantity, basketState.Items?[0].Quantity);
        Assert.Equal(sku2, basketState.Items?[1].SKU);
        Assert.Equal(quantity, basketState.Items?[1].Quantity);
    }
}