using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using HoaHoeHoaSoi.Helpers;
using HoaHoeHoaSoi.Model;
using HoaHoeHoaSoi.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace HoaHoeHoaSoi.Pages
{
	public class SignUpModel : PageModel
    {
        [BindProperty]
        [Required]
        public string Fullname { get; set; }
        [BindProperty]
        [Required]
        [MinLength(5)]
        [MaxLength(10)]
        public string Username { get; set; }
        [BindProperty]
        [Required]
        [MinLength(5)]
        [MaxLength(10)]
        public string Password { get; set; }

        public string ErrorMsg { get; set; } = string.Empty;
        public bool IsSuccess { get; set; } = false;

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if(!ModelState.IsValid)
            {
                ErrorMsg = Resources.InValidSignupMsg;
                return Page();
            }

            using (SqlConnection connection = HoaDBContext.GetSqlConnection())
            {
                connection.Open();
                string sql = "SELECT Id FROM UserInfo where Username = @Username";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Username", Username);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ErrorMsg = Resources.ExistedSignUpMsg;
                            return Page();
                        }
                    }                    

                    command.CommandText = "INSERT INTO [UserInfo] (Username, Password, Name) Values(@Username, @Password, @Name)";
                    command.Parameters.AddWithValue("@Password", Password);
                    command.Parameters.AddWithValue("@Name", Fullname);

                    int modified = command.ExecuteNonQuery();
                    if (modified == 0)
                        ErrorMsg = Resources.SignUpFailed;
                    else
                        IsSuccess = true;
                }
            }

            return Page();
        }
    }
}
