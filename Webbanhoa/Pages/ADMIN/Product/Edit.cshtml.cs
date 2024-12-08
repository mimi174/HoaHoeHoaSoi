using HoaHoeHoaSoi.Helpers;
using HoaHoeHoaSoi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace HoaHoeHoaSoi.Pages.ADMIN.Product {
    public class EditModel : ProductPageModel {
        public ProductViewModel Product { get; set; }
        [BindProperty]
        public string CurrentImg { get; set; }
        [BindProperty]
        public int Id { get; set; }

        public void OnGet(int id) {
            string name = HttpContext.Session.GetString("Name");
            TempData["Name"] = name;
            LoadProduct(id);

        }

        private void LoadProduct(int id) {
            using (var connection = HoaDBContext.GetSqlConnection()) {
                connection.Open();
                string command = $"SELECT Id, Name, Price, Img, Description FROM Products Where Id = {id}";
                using (var sqlCommand = new SqlCommand(command, connection)) {
                    using (var reader = sqlCommand.ExecuteReader()) {
                        if (reader.Read()) {
                            Product = new ProductViewModel();

                            Product.Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                            Product.Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                            Product.Price = reader.IsDBNull(2) ? 0 : reader.GetDouble(2);
                            Product.Img = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                            Product.Description = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                        }
                    }
                }
            }
        }

        public IActionResult OnPost() {
            ModelState.Remove("Description");
            ModelState.Remove("CurrentImg");
            string imgValue = ConvertImgToBase64();
            if (string.IsNullOrEmpty(imgValue))
                imgValue = CurrentImg;

            ModelState.Remove("Img");
            if (!ModelState.IsValid) {
                Product = new ProductViewModel {
                    Id = Id,
                    Name = Name,
                    Price = Price,
                    Img = imgValue,
                    Description = Description
                };
                return Page();
            }

            using (var connection = HoaDBContext.GetSqlConnection()) {
                connection.Open();

                string command = $"UPDATE Products SET Name = N'{Name}', Price = {Price}, Img = '{imgValue}', Description = '{Description}' WHERE Id = {Id}";
                using (var sqlCommand = new SqlCommand(command, connection)) {
                    sqlCommand.ExecuteNonQuery();
                }
            }

            return Redirect("/ADMIN/Product");
        }
    }
}
