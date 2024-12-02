using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using HoaHoeHoaSoi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HoaHoeHoaSoi.Pages
{
	public class ChangePassModel : PageModel
    {
        [BindProperty]
        public string Id { get; set; }

        [BindProperty]
        public string Avatar { get; set; }

        [BindProperty]
        [Required]
        public string CurrentPassword { get; set; }
        [BindProperty]
        [Required]
        [MinLength(5)]
        [MaxLength(10)]
        public string NewPassword { get; set; }
        [BindProperty]
        [Required]
        public string VerifiedPassword { get; set; }
        [BindProperty]
        public string Username { get; set; }
        private IEnumerable<ModelErrorCollection> Errors { get; set; }
        public string ErrorMsg { get; set; } = string.Empty;
        public bool IsSuccess = false;
        public void OnGet(string id)
        {
            Id = id;
            try
            {
                using (SqlConnection connection = HoaDBContext.GetSqlConnection())
                {
                    connection.Open();
                    string sql = "SELECT Avatar, Username FROM UserInfo where Id=@Id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", Id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Avatar = reader.GetString(0);
                                Username = reader.GetString(1);
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

        private string GetUserPassword()
        {
            string result = string.Empty;
            try
            {
                using (SqlConnection connection = HoaDBContext.GetSqlConnection())
                {
                    connection.Open();
                    string sql = "SELECT Password FROM UserInfo where Id=@Id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", Id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                result = reader.GetString(0);
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

            if (string.IsNullOrEmpty(result))
                throw new Exception("Can't get user password");
            return result;
        }

        public IActionResult OnPost()
        {
            ModelState.Remove("Avatar");
            ModelState.Remove("Username");
            if (!ModelState.IsValid)
            {
                Errors = ModelState.Where(x => x.Value != null && x.Value.Errors.Count() > 0).Select(x => x.Value.Errors);
                foreach (var error in Errors)
                {
                    ErrorMsg += string.Join("<br/>", error.Where(e => !string.IsNullOrEmpty(e.ErrorMessage)).Select(e => e.ErrorMessage)) + "<br/>";
                }
            }

            var password = GetUserPassword();
            if (string.IsNullOrEmpty(ErrorMsg))
            {
                if (CurrentPassword != password)
                {
                    ErrorMsg = "Current password is wrong";
                }
                else if (NewPassword != VerifiedPassword)
                {
                    ErrorMsg = "Verify password not match";
                }
            }

            if (!string.IsNullOrEmpty(ErrorMsg)) 
            { 
                return Page();
            }

            try
            {
                using (SqlConnection connection = HoaDBContext.GetSqlConnection())
                {
                    connection.Open();

                    string sql = "UPDATE UserInfo SET password=@password WHERE Id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@password", NewPassword);
                        command.Parameters.AddWithValue("@id", Id);

                        command.ExecuteNonQuery();
                        IsSuccess = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            return Page();
        }
    }
}
