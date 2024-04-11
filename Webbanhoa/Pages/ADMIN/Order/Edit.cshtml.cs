using HoaHoeHoaSoi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace HoaHoeHoaSoi.Pages.ADMIN.Order {
    public class EditModel : PageModel {
        [BindProperty]
        public int OrderLineID { get; set; }

        [BindProperty]
        public string ProductName { get; set; }

        [BindProperty]
        public double Price { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [BindProperty]
        public int Quantity { get; set; }


        public IActionResult OnGet(int eid) {
            string name = HttpContext.Session.GetString("Name");
            TempData["Name"] = name;
            int orderId = -1;
            string jsonStr = HttpContext.Session.GetString("OrderId");
            if (!string.IsNullOrEmpty(jsonStr)) {
                orderId = JsonConvert.DeserializeObject<int>(jsonStr);
                TempData["orderId"] = orderId;
            }
            LoadOrderLine(eid);
            return Page();
        }

        public IActionResult OnPost() {
            int orderId = -1;
            string jsonStr = HttpContext.Session.GetString("OrderId");
            if (!string.IsNullOrEmpty(jsonStr)) {
                orderId = JsonConvert.DeserializeObject<int>(jsonStr);
            }
            if (!ModelState.IsValid) {
                return Page();
            }

            UpdateOrderLine();

            return RedirectToPage($"/ADMIN/Order/Detail", new { id = orderId });
        }

        private void LoadOrderLine(int eid) {
            int orderId = -1;
            string jsonStr = HttpContext.Session.GetString("OrderId");
            if (!string.IsNullOrEmpty(jsonStr)) {
                orderId = JsonConvert.DeserializeObject<int>(jsonStr);
            }
            using (var connection = HoaDBContext.GetSqlConnection()) {
                connection.Open();
                string command = $@"SELECT OL.Id, P.[Name], OL.Price, OL.Quantity " +
                    $"FROM Order_Line AS OL " +
                    $"INNER JOIN Products AS P ON OL.Products_Id = P.Id " +
                    $"WHERE OL.Ordered_Id = {orderId} AND OL.Id = {eid}";
                using (var sqlCommand = new SqlCommand(command, connection)) {
                    using (var reader = sqlCommand.ExecuteReader()) {
                        if (reader.Read()) {
                            OrderLineID = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                            ProductName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                            Price = reader.IsDBNull(2) ? 0 : reader.GetDouble(2);
                            Quantity = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                        }
                    }
                }
            }
        }
        private void UpdateOrderLine() {
            using (var connection = HoaDBContext.GetSqlConnection()) {
                connection.Open();
                string feedbackCommandText = $"UPDATE Order_Line SET Quantity = @Quantity WHERE Id = @OrderLineID";
                using (var feedbackCommand = new SqlCommand(feedbackCommandText, connection)) {
                    feedbackCommand.Parameters.AddWithValue("@Quantity", Quantity);
                    feedbackCommand.Parameters.AddWithValue("@OrderLineID", OrderLineID);
                    feedbackCommand.ExecuteNonQuery();
                }
            }
        }
    }
}

