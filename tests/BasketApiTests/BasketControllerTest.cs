using Microsoft.AspNetCore.Mvc.Testing;
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
        public async Task GetRequestNoBasket()
        {
            var result = await GetReturnsStatus("Basket/empty", HttpStatusCode.BadRequest);

            const string expected = "No basket found with id empty";
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task AddInvalidSku()
        {
            var result = await PostRequestRequestReturnsStatus("Basket/one/add", "{\"sku\": \"string\",\"quantity\": 1}", HttpStatusCode.BadRequest);
            Assert.Equal("No product matching the sku \"string\" exists", result);
        }

        [Fact]
        public async Task AddEmptySku()
        {
            await PostRequestRequestReturnsStatus("Basket/one/add", "{\"quantity\": 1}", HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task AddEmptyQuantity()
        {
            await PostRequestRequestReturnsStatus("Basket/one/add", "{\"sku\": \"sku1\"}", HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task AddOneItemRequest()
        {
            var result = await PostRequestRequestReturnsStatus("Basket/2/add", "{\"sku\": \"sku1\",\"quantity\": 1}", HttpStatusCode.OK);
            const string expected =
                "{\"id\":\"2\",\"items\":[{\"sku\":\"sku1\",\"quantity\":1,\"price\":12.30,\"total\":12.30,\"discount\":1.4760,\"discountDescription\":\"12% of Alpha's\"}],\"subTotal\":12.30,\"discount\":1.4760,\"discountedTotal\":10.8240}";
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task AddTwoItemRequest()
        {
            await PostRequestRequestReturnsStatus("Basket/3/add", "{\"sku\": \"sku1\",\"quantity\": 1}", HttpStatusCode.OK);
            var result = await PostRequestRequestReturnsStatus("Basket/3/add", "{\"sku\": \"sku1\",\"quantity\": 1}", HttpStatusCode.OK);

            const string expected =
                "{\"id\":\"3\",\"items\":[{\"sku\":\"sku1\",\"quantity\":2,\"price\":12.30,\"total\":24.60,\"discount\":2.9520,\"discountDescription\":\"12% of Alpha's\"}],\"subTotal\":24.60,\"discount\":2.9520,\"discountedTotal\":21.6480}";

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task AddMultipleItemRequest()
        {
            await PostRequestRequestReturnsStatus("Basket/4/add", "{\"sku\": \"sku1\",\"quantity\": 1}", HttpStatusCode.OK);
            await PostRequestRequestReturnsStatus("Basket/4/add", "{\"sku\": \"sku1\",\"quantity\": 4}", HttpStatusCode.OK);
            await PostRequestRequestReturnsStatus("Basket/4/add", "{\"sku\": \"sku2\",\"quantity\": 3}", HttpStatusCode.OK);
            await PostRequestRequestReturnsStatus("Basket/4/add", "{\"sku\": \"sku2\",\"quantity\": 1}", HttpStatusCode.OK);
            await PostRequestRequestReturnsStatus("Basket/4/add", "{\"sku\": \"sku3\",\"quantity\": 10}", HttpStatusCode.OK);

            var result = await GetReturnsStatus("Basket/4", HttpStatusCode.OK);

            const string expected =
                "{\"id\":\"4\",\"items\":" +
                "[{\"sku\":\"sku1\",\"quantity\":5,\"price\":12.30,\"total\":61.50,\"discount\":7.3800,\"discountDescription\":\"12% of Alpha's\"}," +
                "{\"sku\":\"sku2\",\"quantity\":4,\"price\":100.99,\"total\":403.96,\"discount\":100.99,\"discountDescription\":\"Buy 3 Betas get 1 free\"}," +
                "{\"sku\":\"sku3\",\"quantity\":10,\"price\":50.99,\"total\":509.90,\"discount\":0,\"discountDescription\":null}]," +
                "\"subTotal\":975.36,\"discount\":108.3700,\"discountedTotal\":866.9900}";

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task RemoveItemNoBasket()
        {
            var result = await PostRequestRequestReturnsStatus("Basket/re/remove", "{\"sku\": \"sku1\",\"quantity\": 1}", HttpStatusCode.BadRequest);

            const string expected = "No basket found with id re";
            Assert.Equal(expected, result);
        }


        [Fact]
        public async Task RemoveInvalidSku()
        {
            var result = await PostRequestRequestReturnsStatus("Basket/r1/remove",
                "{\"sku\": \"string\",\"quantity\": 1}", HttpStatusCode.BadRequest);
            Assert.Equal("No product matching the sku \"string\" exists", result);
        }

        [Fact]
        public async Task RemoveEmptySku()
        {
            await PostRequestRequestReturnsStatus("Basket/r2/remove", "{\"quantity\": 1}",
                HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RemoveEmptyQuantity()
        {
            await PostRequestRequestReturnsStatus("Basket/r2/remove", "{\"sku\": \"sku1\"}",
                HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task RemoveOneItem()
        {
            await PostRequestRequestReturnsStatus("Basket/r3/add", "{\"sku\": \"sku1\",\"quantity\": 2}", HttpStatusCode.OK);

            var result = await PostRequestRequestReturnsStatus("Basket/r3/remove",
                "{\"sku\": \"sku1\",\"quantity\": 1}",
                HttpStatusCode.OK);

            const string expected =
                "{\"id\":\"r3\",\"items\":[{\"sku\":\"sku1\",\"quantity\":1,\"price\":12.30,\"total\":12.30,\"discount\":1.4760,\"discountDescription\":\"12% of Alpha's\"}],\"subTotal\":12.30,\"discount\":1.4760,\"discountedTotal\":10.8240}";

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task RemoveAllItems()
        {
            await PostRequestRequestReturnsStatus("Basket/r4/add", "{\"sku\": \"sku2\",\"quantity\": 2}",
                HttpStatusCode.OK);

            var result = await PostRequestRequestReturnsStatus("Basket/r4/remove",
                "{\"sku\": \"sku2\",\"quantity\": 2}",
                HttpStatusCode.OK);

            const string expected =
                "{\"id\":\"r4\",\"items\":[],\"subTotal\":0,\"discount\":0,\"discountedTotal\":0}";

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task AddAndRemoveItems()
        {
            await PostRequestRequestReturnsStatus("Basket/r5/add", "{\"sku\": \"sku2\",\"quantity\": 2}",
                HttpStatusCode.OK);
            await PostRequestRequestReturnsStatus("Basket/r5/add", "{\"sku\": \"sku1\",\"quantity\": 6}",
                HttpStatusCode.OK);
            await PostRequestRequestReturnsStatus("Basket/r5/add", "{\"sku\": \"sku3\",\"quantity\": 3}",
                HttpStatusCode.OK);

            await PostRequestRequestReturnsStatus("Basket/r5/remove", "{\"sku\": \"sku1\",\"quantity\": 2}",
                HttpStatusCode.OK);
            await PostRequestRequestReturnsStatus("Basket/r5/remove", "{\"sku\": \"sku2\",\"quantity\": 7}",
                HttpStatusCode.BadRequest);
            await PostRequestRequestReturnsStatus("Basket/r5/remove", "{\"sku\": \"sku3\",\"quantity\": 2}",
                HttpStatusCode.OK);

            await PostRequestRequestReturnsStatus("Basket/r5/add", "{\"sku\": \"sku3\",\"quantity\": 4}",
                HttpStatusCode.OK);

            await PostRequestRequestReturnsStatus("Basket/r5/add", "{\"sku\": \"sku2\",\"quantity\": 4}",
                HttpStatusCode.OK);

            await PostRequestRequestReturnsStatus("Basket/r5/add", "{\"sku\": \"sku1\",\"quantity\": 12}",
                HttpStatusCode.OK);

            var result = await GetReturnsStatus("Basket/r5", HttpStatusCode.OK);

            const string expected =
                "{\"id\":\"r5\",\"items\":" +
                "[{\"sku\":\"sku2\",\"quantity\":6,\"price\":100.99,\"total\":605.94,\"discount\":201.98,\"discountDescription\":\"Buy 3 Betas get 1 free\"}," +
                "{\"sku\":\"sku1\",\"quantity\":16,\"price\":12.30,\"total\":196.80,\"discount\":23.6160,\"discountDescription\":\"12% of Alpha's\"}," +
                "{\"sku\":\"sku3\",\"quantity\":5,\"price\":50.99,\"total\":254.95,\"discount\":0,\"discountDescription\":null}]," +
                "\"subTotal\":1057.69,\"discount\":225.5960,\"discountedTotal\":832.0940}";

            Assert.Equal(expected, result);
        }

        public async Task<string> PostRequestRequestReturnsStatus(string url, string request, HttpStatusCode statusCode)
        {
            var content = new StringContent(request);
            content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "application/json");

            var response = await _httpClient.PostAsync(url, content);
            Assert.Equal(statusCode, response.StatusCode);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetReturnsStatus(string url, HttpStatusCode statusCode)
        {
            var response = await _httpClient.GetAsync(url);
            Assert.Equal(statusCode, response.StatusCode);

            return await response.Content.ReadAsStringAsync();
        }
    }
}