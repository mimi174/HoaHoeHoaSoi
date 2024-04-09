using HoaHoeHoaSoi.Helpers;
using HoaHoeHoaSoi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Data.SqlClient;

namespace HoaHoeHoaSoi.Pages {
    public class ADMINModel : PageModel {
        public void OnGet() {
            string name = HttpContext.Session.GetString("Name");
            TempData["Name"] = name;
        }

        public IActionResult OnPost(string username, string password) {
            try {
                Admin admin = Login(username, password);
                string jsonStr = JsonConvert.SerializeObject(admin);
                HttpContext.Session.SetString("admin", jsonStr);
                HttpContext.Session.SetString("Name", admin.Name);
                TempData["AdminName"] = admin.Name;
                if (admin != null) {


                    return RedirectToPage("/ADMIN/HomeAdmin");
                } else {
                    TempData["Error"] = "Login Failed! Please check your username and password!";
                    return Page();
                }
            } catch (Exception ex) {
                TempData["Error"] = "An error occurred while trying to login. Please try again later.";
                return Page();
            }
        }

        private Admin Login(string username, string password) {
            Admin admin = null;

            using (var connection = HoaDBContext.GetSqlConnection()) {
                string query = "SELECT Id, Name, Address, Phone, Username, Password " +
                               "FROM Admin " +
                               "WHERE Username = @Username AND Password = @Password";

                using (var command = new SqlCommand(query, connection)) {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    try {
                        connection.Open();
                        using (var reader = command.ExecuteReader()) {
                            if (reader.Read()) {
                                admin = new Admin(
                                    Convert.ToInt32(reader["Id"]),
                                    Convert.ToString(reader["Name"]),
                                    Convert.ToString(reader["Address"]),
                                    Convert.ToString(reader["Phone"]),
                                    Convert.ToString(reader["Username"]),
                                    Convert.ToString(reader["Password"])
                                );
                            }
                        }
                    } catch (Exception ex) {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }
            return admin;
        }
    }
}
