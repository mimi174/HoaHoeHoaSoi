using HoaHoeHoaSoi.Helpers;
using HoaHoeHoaSoi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace HoaHoeHoaSoi.Pages.ADMIN.Feedback {
    public class IndexModel : PageModel {
        [BindProperty]
        public string Search { get; set; }
        [BindProperty]
        public int DeleteId { get; set; }
        public class FeedbackViewModel {
            public int FeedbackId { get; set; }
            public string CustomerName { get; set; }
            public string Address { get; set; }
            public string Email { get; set; }
            public string Content { get; set; }
            public string Phone { get; set; }
        }
        public List<FeedbackViewModel> FeedbackList { get; set; }


        public IActionResult OnPostDelete(int DeleteId) {
            if (DeleteId > 0) {
                using (var connection = HoaDBContext.GetSqlConnection()) {
                    connection.Open();

                    // Xóa thông tin từ bảng Feedback với điều kiện FeedbackID
                    string deleteFeedbackCommand = $"DELETE FROM Feedback WHERE Id = {DeleteId}";
                    using (var sqlCommand = new SqlCommand(deleteFeedbackCommand, connection)) {
                        sqlCommand.ExecuteNonQuery();
                    }

                    // Xác định CustomerID tương ứng với FeedbackID
                    string selectCustomerIdCommand = $"SELECT CustomerId FROM Feedback WHERE Id = {DeleteId}";
                    int customerId = 0;
                    using (var selectCustomerIdCmd = new SqlCommand(selectCustomerIdCommand, connection)) {
                        using (var reader = selectCustomerIdCmd.ExecuteReader()) {
                            if (reader.Read()) {
                                customerId = reader.GetInt32(0);
                            }
                        }
                    }

                    // Nếu CustomerID được xác định, xóa thông tin từ bảng Customer với điều kiện CustomerID
                    if (customerId > 0) {
                        string deleteCustomerCommand = $"DELETE FROM Customer WHERE Id = {customerId}";
                        using (var deleteCustomerCmd = new SqlCommand(deleteCustomerCommand, connection)) {
                            deleteCustomerCmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            return RedirectToPage("Index");
        }

        public void OnGet() {
            Search = string.Empty;
            var data = Request.Query["search"];
            if (!data.IsNullOrEmpty() && data.FirstOrDefault() != null)
                Search = data.FirstOrDefault();

            FeedbackList = new List<FeedbackViewModel>();

            using (var connection = HoaDBContext.GetSqlConnection()) {
                connection.Open();
                string command = $@"
            SELECT F.Id, C.Name, C.Address, C.Email, C.Phone, F.Content
            FROM Feedback AS F
            INNER JOIN Customer AS C ON F.CustomerId = C.Id
            WHERE F.Content LIKE '%{Search}%' OR F.Id LIKE '%{Search}%' OR C.Name LIKE '%{Search}%' OR C.Address LIKE '%{Search}%' OR C.Email LIKE '%{Search}%' OR C.Phone LIKE '%{Search}%'";
                using (var sqlCommand = new SqlCommand(command, connection)) {
                    using (var reader = sqlCommand.ExecuteReader()) {
                        while (reader.Read()) {
                            var feedback = new FeedbackViewModel();
                            feedback.FeedbackId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                            feedback.CustomerName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                            feedback.Address = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                            feedback.Email = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                            feedback.Phone = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                            feedback.Content = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);

                            FeedbackList.Add(feedback);
                        }
                    }
                }
            }
        }
    }
}