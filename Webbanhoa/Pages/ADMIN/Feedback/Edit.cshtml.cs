using System;
using System.Data.SqlClient;
using HoaHoeHoaSoi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace HoaHoeHoaSoi.Pages.ADMIN.Feedback {
    public class EditModel : PageModel {
        [BindProperty]
        public int FeedbackId { get; set; }

        [BindProperty]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Customer Name is required")]
        [BindProperty]
        public string CustomerName { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [BindProperty]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Invalid Phone Number")]
        [BindProperty]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [BindProperty]
        public string Content { get; set; }

        public IActionResult OnGet(int id) {
            string name = HttpContext.Session.GetString("Name");
            TempData["Name"] = name;
            LoadFeedback(id);
            return Page();
        }

        public IActionResult OnPost() {
            if (!ModelState.IsValid) {
                return Page();
            }

            UpdateFeedback();

            return RedirectToPage("/ADMIN/Feedback/Index");
        }

        private void LoadFeedback(int id) {
            using (var connection = HoaDBContext.GetSqlConnection()) {
                connection.Open();
                string command = $"SELECT Id, Name, Email, Phone, Content " +
                    $"FROM Feedback AS F " +
                    $"WHERE F.Id = {id}";
                using (var sqlCommand = new SqlCommand(command, connection)) {
                    using (var reader = sqlCommand.ExecuteReader()) {
                        if (reader.Read()) {
                            FeedbackId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                            CustomerName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                            Email = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                            PhoneNumber = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                            Content = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                        }
                    }
                }
            }
        }
        private void UpdateFeedback() {
            using (var connection = HoaDBContext.GetSqlConnection()) {
                connection.Open();
                string feedbackCommandText = $"UPDATE Feedback SET Name = @Name, Content = @Content, Email = @Email, Phone = @Phone WHERE Id = @FeedbackId";
                using (var feedbackCommand = new SqlCommand(feedbackCommandText, connection)) {
                    feedbackCommand.Parameters.AddWithValue("@Name", CustomerName);
                    feedbackCommand.Parameters.AddWithValue("@Content", Content);
                    feedbackCommand.Parameters.AddWithValue("@Email", Email);
                    feedbackCommand.Parameters.AddWithValue("@Phone", PhoneNumber);
                    feedbackCommand.Parameters.AddWithValue("@FeedbackId", FeedbackId);
                    feedbackCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
