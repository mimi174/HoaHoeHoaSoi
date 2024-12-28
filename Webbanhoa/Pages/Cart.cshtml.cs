using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using HoaHoeHoaSoi.Helpers;
using HoaHoeHoaSoi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Azure;
using System.Reflection;
using HoaHoeHoaSoi.Properties;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using HoaHoeHoaSoi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HoaHoeHoaSoi.Pages.Shared {
    public class CartModel : PageModel {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CartModel> _logger;

        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Invalid Phone Number")]
        public string Phone { get; set; }

        [BindProperty]
        public string Address { get; set; }

        [Required(ErrorMessage = "Order required")]
        [BindProperty]
        public string CurrentQuantity { get; set; }

        [BindProperty]
        public double TotalAmount { get; set; }
        public List<Products> SelectedProducts { get; set; }

        private MomoAPI _momoAPI { get; set; }

        [BindProperty]
        public PaymentMethod PaymentMethod { get; set; }

        public CartModel(IHttpContextAccessor httpContextAccessor, ILogger<CartModel> logger, IOptions<MomoAPI> momoAPI) {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _momoAPI = momoAPI.Value;
        }

        private UserInfoSession GetUserInfoFromSession()
        {
            var userSessionInfo = HttpContext.Session.GetString(Resources.UserSessionInfo);
            return string.IsNullOrEmpty(userSessionInfo) ? null : JsonConvert.DeserializeObject<UserInfoSession>(userSessionInfo);
        }

        public IActionResult OnGetUpdate(int productId, int quantity)
        {
            var sessionInfo = GetUserInfoFromSession();
            if (sessionInfo == null)
                return Redirect("/Login");

            CartHelper.UpdateCart(productId, quantity, sessionInfo.Id);
            LoadSelectedProducts(sessionInfo.Id);
            return Page();
        }

        public IActionResult OnGet() {
            var sessionInfo = GetUserInfoFromSession();
            if (sessionInfo == null)
                return Redirect("/Login");

            LoadSelectedProducts(sessionInfo.Id);
            if (SelectedProducts == null) {
                SelectedProducts = new List<Products>();
            }

            return Page();
        }

        public IActionResult OnGetDelete(int productId) {
            var sessionInfo = GetUserInfoFromSession();
            if (sessionInfo == null)
                return Redirect("/Login");

            RemoveProductFromCart(productId, sessionInfo.Id);
            LoadSelectedProducts(productId);
            return RedirectToPage("/Cart");
        }

        private void RemoveProductFromCart(int productId, int userId) {
            CartHelper.RemoveProductFromCart(productId, userId);
        }

        private void LoadSelectedProducts(int userId) {
            SelectedProducts = CartHelper.LoadCartInfo(userId);
            TotalAmount = SelectedProducts.Sum(s => s.Price * s.Quantity);
        }

        public async Task<IActionResult> OnPost() {
            var userSessionInfo = HttpContext.Session.GetString(Resources.UserSessionInfo);
            if (string.IsNullOrEmpty(userSessionInfo))
            {
                return Redirect("/Login");
            }
            var userInfo = JsonConvert.DeserializeObject<UserInfoSession>(userSessionInfo);

            if (!ModelState.IsValid) {
                return Page();
            }

            try {
                string momoOrderId = string.Empty;
                string payURL = string.Empty;
                if(PaymentMethod == PaymentMethod.Momo)
                {
                    using(var ctx = new HoaHoeHoaSoiContext())
                    {
                        var cart = CartHelper.GetCartByUserId(userInfo.Id);
                        cart.Total = TotalAmount;
                        ctx.Ordereds.Update(cart);
                        ctx.SaveChanges();
                    }

                    var momoOrder = new MomoOrder
                    {
                        CustomerName = userInfo.Name,
                        Total = TotalAmount,
                        OrderInfo = Resources.OrderInfoMessage
                    };
                    var response = await FuncHelpers.CreatePaymentAsync(momoOrder, _momoAPI);
                    payURL = response.PayUrl;
                    momoOrderId = response.OrderId;
                }
                CartHelper.ProcessCartIntoOrder(userInfo.Id, Name, Address, Phone, momoOrderId);

                if (string.IsNullOrEmpty(payURL)) 
                {
                    return Redirect("/Cart");
                }
                else
                {
                    return Redirect(payURL);
                }
            } catch (Exception ex) {
                ModelState.AddModelError(string.Empty, "An error occurred while processing your order. Please try again later.");
                return RedirectToAction("Get");
            }
        }
    }
}
