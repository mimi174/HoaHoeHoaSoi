using HoaHoeHoaSoi.Helpers;
using HoaHoeHoaSoi.Model;
using HoaHoeHoaSoi.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Net;
using System.Numerics;
using System.Reflection;

namespace HoaHoeHoaSoi.Pages
{
    public class MyOrdersModel : PageModel
    {
        private MomoAPI _momoAPI { get; set; }

        public List<Ordered> Ordereds { get; set; }
        [BindProperty]
        public double Total { get; set; }
        [BindProperty]
        public int OrderId { get; set; }    
        public MyOrdersModel(IOptions<MomoAPI> momoAPI)
        {
            _momoAPI = momoAPI.Value;
        }

        public async Task<IActionResult> OnPost()
        {
            var userSessionInfo = HttpContext.Session.GetString(Resources.UserSessionInfo);
            if (string.IsNullOrEmpty(userSessionInfo))
            {
                return Redirect("/Login");
            }
            var userInfo = JsonConvert.DeserializeObject<UserInfoSession>(userSessionInfo);
            var momoOrder = new MomoOrder
            {
                CustomerName = userInfo.Name,
                Total = Total,
                OrderInfo = Resources.OrderInfoMessage
            };

            var response = await FuncHelpers.CreatePaymentAsync(momoOrder, _momoAPI);
            if (string.IsNullOrEmpty(response.PayUrl))
                return Page();

            using (SqlConnection connection = HoaDBContext.GetSqlConnection())
            {
                connection.Open();
                string sql = "Update Ordered SET PaymentStatus = @PaymentStatus, MomoOrderId = @MomoOrderId WHERE Id = @OrderId";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@PaymentStatus", (int)PaymentStatus.UnPaid);
                    command.Parameters.AddWithValue("@MomoOrderId", response.OrderId);
                    command.Parameters.AddWithValue("@OrderId", OrderId);
                    command.ExecuteNonQuery();
                }
            }
            return Redirect(response.PayUrl);
        }

        public void OnGet(string id)
        {
            Ordereds = new List<Ordered>();
            using (SqlConnection connection = HoaDBContext.GetSqlConnection())
            {
                connection.Open();

                string sql = "SELECT Id, Date, Total, PaymentStatus From Ordered Where UserId = @UserId";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Ordered order = null;
                        while (reader.Read())
                        {
                            order = new Ordered();
                            order.Id  = reader.GetInt32(0);
                            order.Date  = DateOnly.FromDateTime(reader.GetDateTime(1));
                            order.Total = reader.GetDouble(2);
                            order.PaymentStatus = (PaymentStatus)reader.GetInt32(3);
                            Ordereds.Add(order);
                        }
                    }
                }

                foreach (Ordered ordered in Ordereds)
                {
                    sql = "SELECT OL.Id, OL.Products_Id, OL.Price, OL.Quantity, P.Name From Order_Line OL" +
                            " INNER JOIN Products P ON P.Id = OL.Products_Id" +
                            " Where Ordered_Id = @OrderId";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@OrderId", ordered.Id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            OrderLine line = null;
                            while (reader.Read())
                            {
                                line = new OrderLine();
                                line.Id = reader.GetInt32(0);
                                line.ProductId = reader.GetInt32(1);
                                line.Price = reader.GetDouble(2);
                                line.Quantity = reader.GetInt32(3);
                                line.ProductName = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                                ordered.Lines.Add(line);
                            }
                        }
                    }
                }
                
            }
        }
    }
}
