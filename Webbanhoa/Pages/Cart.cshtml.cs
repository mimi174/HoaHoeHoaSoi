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

        public CartModel(IHttpContextAccessor httpContextAccessor, ILogger<CartModel> logger, IOptions<MomoAPI> momoAPI) {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _momoAPI = momoAPI.Value;
        }

        public void OnGet() {
            LoadSelectedProducts();
            if (SelectedProducts == null) {
                SelectedProducts = new List<Products>();
            }
        }

        public IActionResult OnGetDelete(int productId) {
            RemoveProductFromCart(productId);
            LoadSelectedProducts();
            return RedirectToPage("/Cart");
        }

        private void RemoveProductFromCart(int productId) {
            var productJson = _httpContextAccessor.HttpContext.Session.GetString("SelectedProducts");
            if (!string.IsNullOrEmpty(productJson)) {
                var products = JsonConvert.DeserializeObject<List<Products>>(productJson);
                var productToRemove = products.FirstOrDefault(p => p.Id == productId);
                if (productToRemove != null) {
                    products.Remove(productToRemove);
                    _httpContextAccessor.HttpContext.Session.SetString("SelectedProducts", JsonConvert.SerializeObject(products));
                }
            }
        }

        private void LoadSelectedProducts() {
            var productJson = _httpContextAccessor.HttpContext.Session.GetString("SelectedProducts");
            SelectedProducts = string.IsNullOrEmpty(productJson)
                ? new List<Products>()
                : JsonConvert.DeserializeObject<List<Products>>(productJson);
            _logger.LogInformation(SelectedProducts.Count.ToString());
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
                var productJson = _httpContextAccessor.HttpContext.Session.GetString("SelectedProducts");
                var selectedProducts = JsonConvert.DeserializeObject<List<Products>>(productJson);
                if (selectedProducts == null || selectedProducts.Count == 0)
                {
                    ModelState.AddModelError(string.Empty, "No products selected.");
                    return Page();
                }

                var momoOrder = new MomoOrder
                {
                    CustomerName = userInfo.Name,
                    Total = TotalAmount,
                    OrderInfo = Resources.OrderInfoMessage
                };
                var response = await FuncHelpers.CreatePaymentAsync(momoOrder, _momoAPI);
                if(response.PayUrl != null)
                {
                    //int customerId;
                    using (var connection = HoaDBContext.GetSqlConnection())
                    {
                        connection.Open();

                        //string insertCustomerQuery = "INSERT INTO Customer (Name, Phone, Address) OUTPUT INSERTED.Id VALUES (@Name, @Phone, @Address)";
                        //using (var insertCustomerCommand = new SqlCommand(insertCustomerQuery, connection))
                        //{
                        //    insertCustomerCommand.Parameters.AddWithValue("@Name", Name);
                        //    insertCustomerCommand.Parameters.AddWithValue("@Phone", Phone);
                        //    insertCustomerCommand.Parameters.AddWithValue("@Address", Address);
                        //    customerId = (int)insertCustomerCommand.ExecuteScalar();
                        //}
                        string insertOrderQuery = "INSERT INTO Ordered (UserId, Date, Total, MomoOrderId) VALUES (@UserId, @Date, @Total, @MomoOrderId)";
                        using (var insertOrderCommand = new SqlCommand(insertOrderQuery, connection))
                        {
                            insertOrderCommand.Parameters.AddWithValue("@UserId", userInfo.Id);
                            insertOrderCommand.Parameters.AddWithValue("@Date", DateTime.Now);
                            insertOrderCommand.Parameters.AddWithValue("@MomoOrderId", response.OrderId);

                            double total = selectedProducts.Sum(product => product.Price);
                            insertOrderCommand.Parameters.AddWithValue("@Total", TotalAmount);

                            insertOrderCommand.ExecuteNonQuery();
                        }
                        string getOrderIDQuery = "SELECT id\r\nFROM Ordered\r\nORDER BY id DESC\r\nOFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY;";
                        int orderId;
                        using (var getOrderIDCommand = new SqlCommand(getOrderIDQuery, connection))
                        {
                            orderId = Convert.ToInt32(getOrderIDCommand.ExecuteScalar());
                        }
                        string[] quantityArray = CurrentQuantity.Split(',');
                        for (int i = 0; i < selectedProducts.Count; i++)
                        {
                            int quantity = int.Parse(quantityArray[i]);
                            string insertOrderLineQuery = "INSERT INTO Order_Line (Ordered_Id, Products_Id, Price, Quantity) VALUES (@OrderedId, @ProductId, @Price, @Quantity)";
                            using (var insertOrderLineCommand = new SqlCommand(insertOrderLineQuery, connection))
                            {
                                insertOrderLineCommand.Parameters.AddWithValue("@OrderedId", orderId);
                                insertOrderLineCommand.Parameters.AddWithValue("@ProductId", selectedProducts[i].Id);
                                insertOrderLineCommand.Parameters.AddWithValue("@Price", selectedProducts[i].Price);
                                insertOrderLineCommand.Parameters.AddWithValue("@Quantity", quantity);
                                insertOrderLineCommand.ExecuteNonQuery();
                            }
                        }
                    }
                    _httpContextAccessor.HttpContext.Session.Remove("SelectedProducts");
                    TempData["SuccessMessage"] = "Your order has been successfully placed.";
                }
                return Redirect(response.PayUrl);
            } catch (Exception ex) {
                ModelState.AddModelError(string.Empty, "An error occurred while processing your order. Please try again later.");
                return Page();
            }
        }
    }
}
