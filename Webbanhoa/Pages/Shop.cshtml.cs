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
using HoaHoeHoaSoi.Properties;

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
            try {
                using (SqlConnection connection = HoaDBContext.GetSqlConnection()) {
                    connection.Open();
                    string sql = "SELECT Id, Name, Price, Img FROM Products";
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
            var userSessionInfo = HttpContext.Session.GetString(Resources.UserSessionInfo);
            if (string.IsNullOrEmpty(userSessionInfo))
            {
                return Redirect("/Login");
            }

            Products product = null;
            var userInfoSession = JsonConvert.DeserializeObject<UserInfoSession>(userSessionInfo);

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

                if (product == null) {
                    _logger.LogWarning($"Product with ID {productId} not found.");
                    return RedirectToPage("Shop");
                }

                CartHelper.AddToCard(productId, 1, userInfoSession.Id, product.Price);

                return RedirectToPage("Shop");
            } catch (Exception ex) {
                _logger.LogError(ex, "Error fetching product from database.");
                throw;
            }
        }
    }
}


