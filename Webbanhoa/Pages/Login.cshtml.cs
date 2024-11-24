using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HoaHoeHoaSoi.Helpers;
using HoaHoeHoaSoi.Model;
using HoaHoeHoaSoi.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace HoaHoeHoaSoi.Pages
{
	public class LoginModel : PageModel
    {
        public bool IsLoginFail { get; set; } = false;
        public bool IsValid { get; set; } = true;

        [BindProperty]
        [Required]
        public string Username { get; set; }
        [BindProperty]
        [Required]
        public string Password { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString(Resources.UserSessionInfo) != null)
            {
                return Redirect("/");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                IsValid = false;
                return Page();
            }

            using (SqlConnection connection = HoaDBContext.GetSqlConnection())
            {
                connection.Open();
                string sql = "SELECT Id, Name FROM UserInfo where Username = @Username and Password = @Password";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Username", Username);
                    command.Parameters.AddWithValue("@Password", Password);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var sessionInfo = new UserInfoSession
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                            };

                            HttpContext.Session.SetString(Resources.UserSessionInfo, JsonConvert.SerializeObject(sessionInfo));

                            return Redirect("/");
                        }

                        IsLoginFail = true;
                    }
                }
            }
            return Page();
        }
    }
}
