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
        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        private readonly ILogger<IndexModel> _logger;
        public bool hasData = false;
        public IndexModel(ILogger<IndexModel> logger) {
            _logger = logger;
        }

        public void OnGet() {
            string name = HttpContext.Session.GetString("Name");
            TempData["Name"] = name;
        }

        public IActionResult OnPost() {
            if (!ModelState.IsValid) {
                return Page();
            }

            try {
                using (var connection = HoaDBContext.GetSqlConnection()) {
                    connection.Open();


                    string insertCustomerQuery = "INSERT INTO Customer (Name, Address, Phone, Email) OUTPUT INSERTED.Id VALUES (@Name, @Address, @Phone, @Email)";
                    int customerId;
                    using (var insertCustomerCommand = new SqlCommand(insertCustomerQuery, connection)) {
                        insertCustomerCommand.Parameters.AddWithValue("@Name", Name);
                        insertCustomerCommand.Parameters.AddWithValue("@Address", Address);
                        insertCustomerCommand.Parameters.AddWithValue("@Phone", Phone);
                        insertCustomerCommand.Parameters.AddWithValue("@Email", Email);
                        customerId = (int)insertCustomerCommand.ExecuteScalar();
                    }


                    string insertFeedbackQuery = "INSERT INTO Feedback (CustomerId, Content) VALUES (@CustomerId, @Content)";
                    using (var insertFeedbackCommand = new SqlCommand(insertFeedbackQuery, connection)) {
                        insertFeedbackCommand.Parameters.AddWithValue("@CustomerId", customerId);
                        insertFeedbackCommand.Parameters.AddWithValue("@Content", Content);
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
