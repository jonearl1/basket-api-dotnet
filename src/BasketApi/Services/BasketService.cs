using BasketApi.Controllers;
using BasketApi.Models;
using Storage;

namespace BasketApi.Services
{
    public class BasketService
    {
        private readonly BasketState _basketState = new();
        public BasketState GetBasket(string id)
        {
            return _basketState;
        }

        public void AddToBasket(string basketId, string sku1, int quantity)
        {
            
            _basketState.Id = basketId;
            _basketState.AddItem(sku1, quantity);
        }
    }
}
