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
            public string Email { get; set; }
            public string Content { get; set; }
            public string Phone { get; set; }
        }
        public List<FeedbackViewModel> FeedbackList { get; set; }

        public IActionResult OnPostDelete(int DeleteId) {
            if (DeleteId > 0) {
                using (var connection = HoaDBContext.GetSqlConnection()) {
                    connection.Open();
                    string deleteFeedbackCommand = $"DELETE FROM Feedback WHERE Id = {DeleteId}";
                    using (var sqlCommand = new SqlCommand(deleteFeedbackCommand, connection)) {
                        sqlCommand.ExecuteNonQuery();
                    }
                }
            }
            return RedirectToPage("Index");
        }

        public void OnGet() {
            string name = HttpContext.Session.GetString("Name");
            TempData["Name"] = name;
            Search = string.Empty;
            var data = Request.Query["search"];
            if (!data.IsNullOrEmpty() && data.FirstOrDefault() != null)
                Search = data.FirstOrDefault();

            FeedbackList = new List<FeedbackViewModel>();

            using (var connection = HoaDBContext.GetSqlConnection()) {
                connection.Open();
                string command = $@"
            SELECT Id, Name, Email, Phone, Content
            FROM Feedback 
            WHERE Content LIKE '%{Search}%' OR Id LIKE '%{Search}%' OR Name LIKE '%{Search}%' OR Email LIKE '%{Search}%' OR Phone LIKE '%{Search}%'";
                using (var sqlCommand = new SqlCommand(command, connection)) {
                    using (var reader = sqlCommand.ExecuteReader()) {
                        while (reader.Read()) {
                            var feedback = new FeedbackViewModel();
                            feedback.FeedbackId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                            feedback.CustomerName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                            feedback.Email = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                            feedback.Phone = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                            feedback.Content = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);

                            FeedbackList.Add(feedback);
                        }
                    }
                }
            }
        }
    }
}