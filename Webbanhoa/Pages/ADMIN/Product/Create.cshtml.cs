using HoaHoeHoaSoi.Helpers;
using HoaHoeHoaSoi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace HoaHoeHoaSoi.Pages.ADMIN.Product {
    public class CreateModel : ProductPageModel {
        ProductViewModel productViewModel = new ProductViewModel();

        public void OnGet() {
            string name = HttpContext.Session.GetString("Name");
            TempData["Name"] = name;
        }

        public IActionResult OnPost() {
            if (!ModelState.IsValid) {
                return Page();
            }

            string imgValue = ConvertImgToBase64();

            using (var connection = HoaDBContext.GetSqlConnection()) {
                connection.Open();
                var command = $"INSERT INTO Products(Name, Price, Img) VALUES(N'{Name}', {Price}, '{imgValue}')";

                using (var sqlCommand = new SqlCommand(command, connection)) {
                    sqlCommand.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Get");
        }
    }
}
