using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Diagnostics;
using WebMvc.Services;
using WebMvc.Models;
using WebMvc.Models.CartModels;
using Polly.CircuitBreaker;
using WebMvc.Services;
using WebMvc.Models;

namespace WebMvc.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        
        private readonly ICartService _cartService;
        private readonly ICatalogService _catalogService;
        private readonly IIdentityService<ApplicationUser> _identityService;

        public CartController(IIdentityService<ApplicationUser> identityService, ICartService cartService, ICatalogService catalogService)
        {
            _identityService = identityService;
            _cartService = cartService;
            _catalogService = catalogService;



        }
        public    IActionResult  Index()
        {
            //try
            //{

            //    var user = _identityService.Get(HttpContext.User);
            //    var cart = await _cartService.GetCart(user);


            //    return View();
            //}
            //catch (BrokenCircuitException)
            //{
            //    // Catch error when CartApi is in circuit-opened mode                 
            //    HandleBrokenCircuitException();
            //}

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Dictionary<string, int> quantities, string action)
        {
            if (action == "[ Checkout ]")
            {
                return RedirectToAction("Create", "Order");
            }


            try
            {
                var user = _identityService.Get(HttpContext.User);
                var basket = await _cartService.SetQuantities(user, quantities);
                var vm = await _cartService.UpdateCart(basket);

            }
            catch (BrokenCircuitException)
            {
                // Catch error when CartApi is in open circuit  mode                 
                HandleBrokenCircuitException();
            }

            return View();

        }

            public async Task<IActionResult> AddToCart(CatalogItem productDetails)
        {
            try
            {
                if (productDetails.Id != null)
                {
                    var user = _identityService.Get(HttpContext.User);
                    var product = new CartItem()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Quantity = 1,
                        ProductName = productDetails.Name,
                        PictureUrl = productDetails.PictureUrl,
                        UnitPrice = productDetails.Price,
                        ProductId = productDetails.Id
                    };
                    await _cartService.AddItemToCart(user, product);
                }
                return RedirectToAction("Index", "Catalog");
            }
            catch (BrokenCircuitException)
            {
                // Catch error when CartApi is in circuit-opened mode                 
                HandleBrokenCircuitException();
            }

            return RedirectToAction("Index", "Catalog");

        }
        //public async Task WriteOutIdentityInfo()
        //{
        //    var identityToken =
        //        await HttpContext.Authentication.
        //         GetAuthenticateInfoAsync(OpenIdConnectParameterNames.IdToken);
        //    Debug.WriteLine($"Identity Token: {identityToken}");
        //    foreach (var claim in User.Claims)
        //    {
        //        Debug.WriteLine($"Claim Type: {claim.Type} - Claim Value : {claim.Value}");
        //    }

        //}

        private void HandleBrokenCircuitException()
        {
            TempData["BasketInoperativeMsg"] = "cart Service is inoperative, please try later on. (Business Msg Due to Circuit-Breaker)";
        }

    }
}