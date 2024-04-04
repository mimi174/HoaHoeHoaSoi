using HoaHoeHoaSoi.Helpers;
using HoaHoeHoaSoi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;

namespace HoaHoeHoaSoi.Pages.ADMIN.Product
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Search { get; set; }
        public List<ProductViewModel> Products = new List<ProductViewModel>();

        [BindProperty]
        public int DeleteId { get; set; }

        public IActionResult OnPostDelete()
        {
            if(DeleteId > 0)
            {
                using(var connection = HoaDBContext.GetSqlConnection())
                {
                    connection.Open();
                    string command = $"DELETE FROM Products Where Id = {DeleteId}";
                    using(var sqlCommand = new SqlCommand(command, connection))
                    {
                        sqlCommand.ExecuteNonQuery();
                    }
                }
            }

            return RedirectToPage("Index");
        }

        public void OnGet()
        {
            Search = string.Empty;
            var data = Request.Query["search"];
            if (!data.IsNullOrEmpty() && data.FirstOrDefault() != null)
                Search = data.FirstOrDefault();

            using(var connection = HoaDBContext.GetSqlConnection())
            {
                connection.Open();
                string command = $"SELECT Id, Name, Price, Img FROM Products WHERE Name like '%{Search}%'";
                using(var sqlCommand = new SqlCommand(command, connection))
                {
                    using(var  reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var product = new ProductViewModel();
                            product.Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                            product.Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                            product.Price = reader.IsDBNull(2) ? 0 : reader.GetDouble(2);
                            product.Img = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);

                            Products.Add(product);
                        }
                    }
                }
            }
        }
    }
}
