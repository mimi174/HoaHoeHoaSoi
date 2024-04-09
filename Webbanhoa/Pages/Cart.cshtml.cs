using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HoaHoeHoaSoi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace HoaHoeHoaSoi.Pages.Shared {
    public class CartModel : PageModel {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CartModel> _logger;
        public List<Products> SelectedProducts { get; set; }

        public CartModel(IHttpContextAccessor httpContextAccessor, ILogger<CartModel> logger) {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public void OnGet() {
            string name = HttpContext.Session.GetString("Name");
            TempData["Name"] = name;
            var productJson = _httpContextAccessor.HttpContext.Session.GetString("SelectedProducts");
            SelectedProducts = string.IsNullOrEmpty(productJson)
                ? new List<Products>()
                : JsonConvert.DeserializeObject<List<Products>>(productJson);
            _logger.LogInformation(SelectedProducts.Count.ToString());
        }
    }
}
