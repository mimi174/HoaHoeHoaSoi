using HoaHoeHoaSoi.Helpers;
using HoaHoeHoaSoi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace Webbanhoa.Pages {
    public class IndexModel : PageModel {
        [BindProperty]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Invalid Phone Number")]
        public string Phone { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }


        private readonly ILogger<IndexModel> _logger;
        public bool hasData = false;
        public IndexModel(ILogger<IndexModel> logger) {
            _logger = logger;
        }

        public void OnGet() {
        }

        public IActionResult OnPost() {
            if (!ModelState.IsValid) {
                return Page();
            }

            try {
                using (var connection = HoaDBContext.GetSqlConnection()) {
                    connection.Open();

                    string insertFeedbackQuery = "INSERT INTO Feedback (Name, Content, Email, Phone) VALUES (@Name, @Content, @Email, @Phone)";
                    using (var insertFeedbackCommand = new SqlCommand(insertFeedbackQuery, connection)) {
                        insertFeedbackCommand.Parameters.AddWithValue("@Name", Name);
                        insertFeedbackCommand.Parameters.AddWithValue("@Content", Content);
                        insertFeedbackCommand.Parameters.AddWithValue("@Email", Email);
                        insertFeedbackCommand.Parameters.AddWithValue("@Phone", Phone);
                        insertFeedbackCommand.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = "Your feedback has been successfully sent.";
                return RedirectToPage("/Index");
            } catch (Exception ex) {
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again later.");
                return Page();
            }
        }
    }
}
