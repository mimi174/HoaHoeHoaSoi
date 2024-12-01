using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using HoaHoeHoaSoi.Helpers;
using HoaHoeHoaSoi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HoaHoeHoaSoi.Pages
{
	public class UserInfoModel : PageModel
    {
        [BindProperty]
        public string Id { get; set; }

        [BindProperty]
        [Required]
        [MaxLength(20)]
        [MinLength(5)]
        public string Fullname { get; set; }
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Address { get; set; }
        [BindProperty]
        public string Phone { get; set; }
        [BindProperty]
        public string Avatar { get; set; }
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public bool Gender { get; set; }
        [BindProperty]
        public DateTime? DOB { get; set; }
        [BindProperty]
        [ImageExtenstion]
        public IFormFile FileAvatar { get; set; }
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
                    string sql = "SELECT * FROM UserInfo where Id=@Id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", Id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())   
                            {          
                                Username = reader.GetString(1);
                                Fullname = reader.GetString(3);
                                DOB = reader.IsDBNull(4) ? null : reader.GetDateTime(4);
                                Address = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                                Phone = reader.IsDBNull(6) ? string.Empty : reader.GetString(6);
                                Avatar = reader.IsDBNull(7) ? string.Empty : reader.GetString(7);
                                Gender = reader.IsDBNull(8) ? false : reader.GetBoolean(8);
                                Email = reader.IsDBNull(9) ? string.Empty : reader.GetString(9);
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

        public IActionResult OnPost()
        {
            ModelState.Remove("Avatar");
            ModelState.Remove("FileAvatar");
            if (!ModelState.IsValid)
            {
                Errors =  ModelState.Where(x => x.Value != null && x.Value.Errors.Count() > 0).Select(x => x.Value.Errors);
                foreach(var error in Errors)
                {
                    ErrorMsg += string.Join("<br/>", error.Where(e => !string.IsNullOrEmpty(e.ErrorMessage)).Select(e => e.ErrorMessage)) + "<br/>";
                }
                return Page();
            }

            if(FileAvatar != null)
            {
                Avatar = FuncHelpers.ConvertImgToBase64(FileAvatar);
            }

            try
            {
                using (SqlConnection connection = HoaDBContext.GetSqlConnection())
                {
                    connection.Open();

                    string sql = "UPDATE UserInfo SET mail=@email,gender=@gender,dob=@dob,address=@address,phone=@phone,name=@name,avatar=@avatar WHERE Id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", Email);
                        command.Parameters.AddWithValue("@Name", Fullname);
                        command.Parameters.AddWithValue("@gender", Gender ? 1 : 0);
                        command.Parameters.AddWithValue("@DOB", DOB);
                        command.Parameters.AddWithValue("@address", Address);
                        command.Parameters.AddWithValue("@phone", Phone);
                        command.Parameters.AddWithValue("@avatar", string.IsNullOrEmpty(Avatar) ? "" : Avatar);
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

