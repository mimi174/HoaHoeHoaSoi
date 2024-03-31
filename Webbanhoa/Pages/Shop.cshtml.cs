using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HoaHoeHoaSoi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Webbanhoa.Pages.Shared
{
    public class ShopModel : PageModel
    {
        public List<Products> listProducts = new List<Products>();

        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=DESKTOP-0BD8UOC;Initial Catalog=HoaHoeHoaSoi;Persist Security Info=True;User ID=sa;Password=123456;Trusted_Connection=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Products";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var Products = new Products();
                                Products.Id = reader.GetInt32(0);
                                Products.Name = reader.GetString(1);
                                Products.Price = reader.GetDouble(2);
                                Products.Img = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                                listProducts.Add(Products);
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
    }
}


