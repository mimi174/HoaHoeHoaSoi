using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HoaHoeHoaSoi.Helpers;
using HoaHoeHoaSoi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Webbanhoa.Pages.Shared;
using HoaHoeHoaSoi.Properties;

namespace HoaHoeHoaSoi.Pages
{
    public class ViewProductModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ShopModel> _logger;
        public Products Product { get; set; }

        public ViewProductModel(IHttpContextAccessor httpContextAccessor, ILogger<ShopModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public void OnGet(int id)
        {
            try
            {
                using (SqlConnection connection = HoaDBContext.GetSqlConnection())
                {
                    connection.Open();
                    string sql = "SELECT Id, Name, Price, Img, Description FROM Products Where Id = @Id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Product = new Products();
                                Product.Id = reader.GetInt32(0);
                                Product.Name = reader.GetString(1);
                                Product.Price = reader.GetDouble(2);
                                Product.Img = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                                Product.Description = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        [HttpPost]
        public IActionResult OnPostAddToCart(int productId)
        {
            var userSessionInfo = HttpContext.Session.GetString(Resources.UserSessionInfo);
            if (string.IsNullOrEmpty(userSessionInfo))
            {
                return Redirect("/Login");
            }
            Products product = null;

            try
            {
                using (SqlConnection connection = HoaDBContext.GetSqlConnection())
                {
                    connection.Open();
                    string sql = "SELECT * FROM Products WHERE Id = @Id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", productId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                product = new Products();
                                product.Id = reader.GetInt32(0);
                                product.Name = reader.GetString(1);
                                product.Price = reader.GetDouble(2);
                                product.Img = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                            }
                        }
                    }
                }

                if (product == null)
                {
                    _logger.LogWarning($"Product with ID {productId} not found.");
                    return RedirectToPage("Shop");
                }

                var productJson = _httpContextAccessor.HttpContext.Session.GetString("SelectedProducts");
                var products = string.IsNullOrEmpty(productJson) ? new List<Products>() : JsonConvert.DeserializeObject<List<Products>>(productJson);

                if (products.Any(p => p.Id == productId))
                {
                    _logger.LogInformation($"Product with ID {productId} already exists in the cart.");
                    return Redirect("/ViewProduct/" + productId);
                }

                products.Add(product);
                _logger.LogInformation(
                    $"Added product with ID {productId} to the cart. There are now {products.Count} product(s) in the cart.");

                _httpContextAccessor.HttpContext.Session.SetString("SelectedProducts",
                    JsonConvert.SerializeObject(products));

                return Redirect("/ViewProduct/" + productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product from database.");
                throw;
            }
        }
    }
}


