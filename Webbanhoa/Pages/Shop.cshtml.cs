using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Webbanhoa.Pages.Shared
{
	public class ShopModel : PageModel
    {
        public List<ClientInfo> listClients = new List<ClientInfo>();

        public void OnGet()
        {
            try
            {
                string connectionString = "Server=localhost;Database=My_store;User Id=sa;Password=Password.1;Trusted_Connection=";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Clients";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var clientInfo = new ClientInfo();
                            clientInfo.Id = "" + reader.GetInt32(0);
                            clientInfo.Name = reader.GetString(1);
                            clientInfo.Email = reader.GetString(2);
                            clientInfo.Phone = reader.GetString(3);
                            clientInfo.Address = reader.GetString(4);
                            clientInfo.Created_at = reader.GetDateTime(5).ToString();
                            clientInfo.Urf = reader.GetString(6);
                            listClients.Add(clientInfo);
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

public class ClientInfo
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Phone { get; set; }

    public string Email { get; set; }

    public string Address { get; set; }

    public string Created_at { get; set; }

    public string Urf { get; set; }
}

