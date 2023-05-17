using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace BasketApiTests
{
    public class BasketControllerTest
    {
        private HttpClient _httpClient;

        public BasketControllerTest()
        {
            var webApplicationFactory = new WebApplicationFactory<Program>();
            _httpClient = webApplicationFactory.CreateDefaultClient();
        }

        [Fact]
        public async Task GetRequestOnEmptyBaskets()
        {
            var response = await _httpClient.GetAsync("Basket/empty");
            var result = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            const string expected = "No basket found with id empty";
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task AddInvalidSku()
        {
            var content = new StringContent("{\"sku\": \"string\",\"quantity\": 1}");
            content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "application/json");

            var response = await _httpClient.PostAsync("Basket/one/add", content);
            var result = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("No product matching the sku \"string\" exists", result);
        }

        [Fact]
        public async Task AddEmptySku()
        {
            var content = new StringContent("{\"quantity\": 1}");
            content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "application/json");

            var response = await _httpClient.PostAsync("Basket/one/add", content);
            var result = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task AddEmptyQuantity()
        {
            var content = new StringContent("{\"sku\": \"sku1\"}");
            content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "application/json");

            var response = await _httpClient.PostAsync("Basket/one/add", content);
            var result = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task AddOneItemRequest()
        {
            var content = new StringContent("{\"sku\": \"sku1\",\"quantity\": 1}");
            content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "application/json");

            var response = await _httpClient.PostAsync("Basket/2/add", content);
            var result = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            const string expected =
                "{\"id\":\"2\",\"items\":[{\"sku\":\"sku1\",\"quantity\":1,\"price\":12.30,\"total\":12.30,\"discount\":1.4760,\"discountDescription\":\"12% of Alpha's\"}],\"subTotal\":12.30,\"discount\":1.4760,\"discountedTotal\":10.8240}";
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task AddTwoItemRequest()
        {
            await AddRequestReturnsOk("Basket/3/add", "{\"sku\": \"sku1\",\"quantity\": 1}");

            var result = await AddRequestReturnsOk("Basket/3/add", "{\"sku\": \"sku1\",\"quantity\": 1}");

            const string expected =
                "{\"id\":\"3\",\"items\":[{\"sku\":\"sku1\",\"quantity\":2,\"price\":12.30,\"total\":24.60,\"discount\":2.9520,\"discountDescription\":\"12% of Alpha's\"}],\"subTotal\":24.60,\"discount\":2.9520,\"discountedTotal\":21.6480}";

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task AddMultipleItemRequest()
        {
            await AddRequestReturnsOk("Basket/4/add", "{\"sku\": \"sku1\",\"quantity\": 1}");
            await AddRequestReturnsOk("Basket/4/add", "{\"sku\": \"sku1\",\"quantity\": 4}");
            await AddRequestReturnsOk("Basket/4/add", "{\"sku\": \"sku2\",\"quantity\": 3}");
            await AddRequestReturnsOk("Basket/4/add", "{\"sku\": \"sku2\",\"quantity\": 1}");
            await AddRequestReturnsOk("Basket/4/add", "{\"sku\": \"sku3\",\"quantity\": 10}");

            var response = await _httpClient.GetAsync("Basket/4");
            var result = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            const string expected =
                "{\"id\":\"4\",\"items\":" +
                "[{\"sku\":\"sku1\",\"quantity\":5,\"price\":12.30,\"total\":61.50,\"discount\":7.3800,\"discountDescription\":\"12% of Alpha's\"}," +
                "{\"sku\":\"sku2\",\"quantity\":4,\"price\":100.99,\"total\":403.96,\"discount\":100.99,\"discountDescription\":\"Buy 3 Betas get 1 free\"}," +
                "{\"sku\":\"sku3\",\"quantity\":10,\"price\":50.99,\"total\":509.90,\"discount\":0,\"discountDescription\":null}]," +
                "\"subTotal\":975.36,\"discount\":108.3700,\"discountedTotal\":866.9900}";
        
            Assert.Equal(expected, result);
        }

        public async Task<string> AddRequestReturnsOk(string url, string request)
        {
            var content = new StringContent(request);
            content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "application/json");

            var response = await _httpClient.PostAsync(url, content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            return await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async Task RemoveItemEmptyBasket()
        {
            var content = new StringContent("{\"sku\": \"sku1\"}");
            content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "application/json");

            var response = await _httpClient.PostAsync("Basket/re/remove", content);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            const string expected = "No basket found with id re";
            Assert.Equal(expected, result);
        }


        [Fact]
        public async Task RemoveInvalidSku()
        {
            var content = new StringContent("{\"sku\": \"string\"}");
            content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "application/json");

            var response = await _httpClient.PostAsync("Basket/r1/remove", content);
            var result = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("No product matching the sku \"string\" exists", result);
        }

        [Fact]
        public async Task RemoveEmptySku()
        {
            var content = new StringContent("{}");
            content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "application/json");

            var response = await _httpClient.PostAsync("Basket/r2/remove", content);
            var result = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }



        // [Fact]
        // public async Task RemoveOneItem()
        // {
        //     await AddRequestReturnsOk("Basket/r3/add", "{\"sku\": \"sku1\",\"quantity\": 2}");
        //
        //     var content = new StringContent("{}");
        //     content.Headers.Remove("Content-Type");
        //     content.Headers.Add("Content-Type", "application/json");
        //
        //     var response = await _httpClient.PostAsync("Basket/r3/remove", content);
        //     var result = await response.Content.ReadAsStringAsync();
        //     Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //
        //     // const string expected =
        //     //     "{\"id\":\"3\",\"items\":[{\"sku\":\"sku1\",\"quantity\":1,\"price\":12.30,\"total\":12.30,\"discount\":1.4760,\"discountDescription\":\"12% of Alpha's\"}],\"subTotal\":12.30,\"discount\":1.4760,\"discountedTotal\":10.8240}";
        //     //
        //     // Assert.Equal(expected, result);
        // }
    }
}