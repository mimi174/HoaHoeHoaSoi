using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HoaHoeHoaSoi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using HoaHoeHoaSoi.Helpers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Webbanhoa.Pages.Shared {
    public class ShopModel : PageModel {
        public List<Products> listProducts = new List<Products>();
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ShopModel> _logger;

        public ShopModel(IHttpContextAccessor httpContextAccessor, ILogger<ShopModel> logger) {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public void OnGet() {
            string name = HttpContext.Session.GetString("Name");
            TempData["Name"] = name;
            try {
                using (SqlConnection connection = HoaDBContext.GetSqlConnection()) {
                    connection.Open();
                    string sql = "SELECT * FROM Products";
                    using (SqlCommand command = new SqlCommand(sql, connection)) {
                        using (SqlDataReader reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                var Products = new Products();
                                Products.Id = reader.GetInt32(0);
                                Products.Name = reader.GetString(1);
                                Products.Price = reader.GetDouble(2);
                                Products.Img = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                                listProducts.Add(Products);
                            }
                        }
                    }
                }

            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        [HttpPost]
        public IActionResult OnPostAddToCart(int productId) {
            Products product = null;

            try {
                using (SqlConnection connection = HoaDBContext.GetSqlConnection()) {
                    connection.Open();
                    string sql = "SELECT * FROM Products WHERE Id = @Id";
                    using (SqlCommand command = new SqlCommand(sql, connection)) {
                        command.Parameters.AddWithValue("@Id", productId);
                        using (SqlDataReader reader = command.ExecuteReader()) {
                            if (reader.Read()) {
                                product = new Products();
                                product.Id = reader.GetInt32(0);
                                product.Name = reader.GetString(1);
                                product.Price = reader.GetDouble(2);
                                product.Img = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                _logger.LogError(ex, "Error fetching product from database.");
                throw;
            }

            if (product == null) {
                _logger.LogWarning($"Product with ID {productId} not found.");
                return RedirectToPage("Shop");
            }

            var productJson = _httpContextAccessor.HttpContext.Session.GetString("SelectedProducts");
            var products = string.IsNullOrEmpty(productJson)
                ? new List<Products>()
                : JsonConvert.DeserializeObject<List<Products>>(productJson);
            products.Add(product);
            _logger.LogInformation(
                $"Added product with ID {productId} to the cart. There are now {products.Count} product(s) in the cart.");
            _httpContextAccessor.HttpContext.Session.SetString("SelectedProducts",
                JsonConvert.SerializeObject(products));

            return RedirectToPage("Shop");
        }
    }
}


