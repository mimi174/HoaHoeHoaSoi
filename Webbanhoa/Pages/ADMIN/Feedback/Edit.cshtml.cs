using System;
using System.Data.SqlClient;
using HoaHoeHoaSoi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace HoaHoeHoaSoi.Pages.ADMIN.Feedback {
    public class EditModel : PageModel {
        //[Required(ErrorMessage = "Customer Name is required")]
        //public string CustomerName { get; set; }

        //[Required(ErrorMessage = "Address is required")]
        //public string Address { get; set; }

        //[Required(ErrorMessage = "Email is required")]
        //[EmailAddress(ErrorMessage = "Invalid Email Address")]
        //public string Email { get; set; }

        //[Required(ErrorMessage = "Phone Number is required")]
        //[RegularExpression(@"^\d{10,11}$", ErrorMessage = "Invalid Phone Number")]
        //public string PhoneNumber { get; set; }

        //[Required(ErrorMessage = "Content is required")]
        //public string Content { get; set; }





        [BindProperty]
        public int FeedbackId { get; set; }

        [BindProperty]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Customer Name is required")]
        [BindProperty]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [BindProperty]
        public string Address { get; set; }

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
                string command = $"SELECT F.Id, C.Id, C.Name AS CustomerName, C.Address, C.Email, C.Phone, F.Content " +
                    $"FROM Feedback AS F " +
                    $"INNER JOIN Customer AS C ON F.CustomerId = C.Id " +
                    $"WHERE F.Id = {id}";
                using (var sqlCommand = new SqlCommand(command, connection)) {
                    using (var reader = sqlCommand.ExecuteReader()) {
                        if (reader.Read()) {
                            FeedbackId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                            CustomerId = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                            CustomerName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                            Address = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                            Email = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                            PhoneNumber = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                            Content = reader.IsDBNull(6) ? string.Empty : reader.GetString(6);
                        }
                    }
                }
            }
        }
        private void UpdateFeedback() {
            using (var connection = HoaDBContext.GetSqlConnection()) {
                connection.Open();
                // Cập nhật thông tin phản hồi
                string feedbackCommandText = $"UPDATE Feedback SET Content = @Content WHERE Id = @FeedbackId";
                using (var feedbackCommand = new SqlCommand(feedbackCommandText, connection)) {
                    feedbackCommand.Parameters.AddWithValue("@Content", Content);
                    feedbackCommand.Parameters.AddWithValue("@FeedbackId", FeedbackId);
                    feedbackCommand.ExecuteNonQuery();
                }

                // Cập nhật thông tin của khách hàng
                string customerCommandText = $"UPDATE Customer SET Name = @CustomerName, Address = @Address, Email = @Email, Phone = @PhoneNumber WHERE Id = @CustomerId";
                using (var customerCommand = new SqlCommand(customerCommandText, connection)) {
                    customerCommand.Parameters.AddWithValue("@CustomerName", CustomerName);
                    customerCommand.Parameters.AddWithValue("@Address", Address);
                    customerCommand.Parameters.AddWithValue("@Email", Email);
                    customerCommand.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);
                    customerCommand.Parameters.AddWithValue("@CustomerId", CustomerId);
                    customerCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
