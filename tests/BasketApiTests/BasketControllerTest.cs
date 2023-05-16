using System;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http.Json;

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
        public async Task InvalidSku()
        {
            var content = new StringContent("{\r\n  \"sku\": \"string\",\r\n  \"quantity\": 0\r\n}");
            content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "application/json");

            var response = await _httpClient.PostAsync("Basket/one/add", content);
            var result = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("No product matching the sku \"string\" exists", result);
        }

        [Fact]
        public async Task EmptySku()
        {
            var content = new StringContent("{\r\n  \"quantity\": 1\r\n}");
            content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "application/json");

            var response = await _httpClient.PostAsync("Basket/one/add", content);
            var result = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("sku must not be null", result);
        }

        [Fact]
        public async Task EmptyQuantity()
        {
            var content = new StringContent("{\r\n  \"sku\": \"sku1\"\r\n}");
            content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "application/json");

            var response = await _httpClient.PostAsync("Basket/one/add", content);
            var result = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("quantity must not be null", result);
        }

        [Fact]
        public async Task ValidAddRequest()
        {
            var content = new StringContent("{\r\n  \"sku\": \"sku1\",\r\n  \"quantity\": 1\r\n}");
            content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "application/json");

            var response = await _httpClient.PostAsync("Basket/1/add", content);
            var result = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            const string expected =
                "{\"id\":\"1\",\"items\":[{\"sku\":\"sku1\",\"quantity\":1,\"price\":12.30,\"total\":12.30,\"discount\":1.4760,\"discountDescription\":\"12% of Alpha's\"}],\"subTotal\":12.30,\"discount\":1.4760,\"discountedTotal\":10.8240}";
        Assert.Equal(expected, result);
        }
    }
}