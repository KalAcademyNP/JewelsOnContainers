using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebMvc.Infrastructure;
using WebMvc.Models;
using WebMvc.Models.OrderModels;
using WebMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace WebMvc.Services
{
    public class OrderService : IOrderService
    {
        private IHttpClient _apiClient;
        private readonly string _remoteServiceBaseUrl;
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly ILogger _logger;
        public OrderService(IOptionsSnapshot<AppSettings> settings, IHttpContextAccessor httpContextAccesor, IHttpClient httpClient, ILoggerFactory logger)
        {
            _remoteServiceBaseUrl = $"{settings.Value.OrderUrl}/api/v1/orders";
            _settings = settings;
            _httpContextAccesor = httpContextAccesor;
            _apiClient = httpClient;
            _logger = logger.CreateLogger<OrderService>();
        }

        async public Task<Order> GetOrder(string id)
        {
            var token = await GetUserTokenAsync();
            var getOrderUri = ApiPaths.Order.GetOrder(_remoteServiceBaseUrl, id);

            var dataString = await _apiClient.GetStringAsync(getOrderUri, token);
            _logger.LogInformation("DataString: " + dataString);
            var response = JsonConvert.DeserializeObject<Order>(dataString);

            return response;
        }

        public async   Task<List<Order>> GetOrders()
        {
            var token = await GetUserTokenAsync();
            var allOrdersUri =ApiPaths.Order.GetOrders(_remoteServiceBaseUrl);

            var dataString = await _apiClient.GetStringAsync(allOrdersUri, token);
            var response = JsonConvert.DeserializeObject<List<Order>>(dataString);

            return response;
        }
      
 
        public async Task<int>  CreateOrder(Order order)
        {
            var token = await GetUserTokenAsync();
          
            var addNewOrderUri = ApiPaths.Order.AddNewOrder(_remoteServiceBaseUrl);
            _logger.LogDebug(" OrderUri " + addNewOrderUri);

            
            var response = await _apiClient.PostAsync(addNewOrderUri, order, token);
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                throw new Exception("Error creating order, try later.");
            }
           
           // response.EnsureSuccessStatusCode();
            var jsonString = response.Content.ReadAsStringAsync();

            jsonString.Wait();
            _logger.LogDebug("response " + jsonString);
            dynamic data = JObject.Parse(jsonString.Result);
            string value = data.orderId;
            return Convert.ToInt32(value);
        }

        async Task<string> GetUserTokenAsync()
        {
            var context = _httpContextAccesor.HttpContext;

            return await context.GetTokenAsync("access_token");
        }


        //public async  Task<List<Order>> GetOrdersByUser(ApplicationUser user)
        //{
        //    var token = await GetUserTokenAsync();
        //    var allMyOrdersUri = ApiPaths.Order.GetOrdersByUser(_remoteServiceBaseUrl,user.Email);

        //    var dataString = await _apiClient.GetStringAsync(allMyOrdersUri, token);
        //    var response = JsonConvert.DeserializeObject<List<Order>>(dataString);

        //    return response;
        //}


    }
}
