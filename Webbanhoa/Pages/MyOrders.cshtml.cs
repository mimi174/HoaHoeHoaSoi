using HoaHoeHoaSoi.Data.Models;
using HoaHoeHoaSoi.Helpers;
using HoaHoeHoaSoi.Model;
using HoaHoeHoaSoi.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Net;
using System.Numerics;
using System.Reflection;
using Ordered = HoaHoeHoaSoi.Model.Ordered;
using OrderLine = HoaHoeHoaSoi.Model.OrderLine;

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
        private readonly IConfiguration _configuration;
        public MyOrdersModel(IOptions<MomoAPI> momoAPI, IConfiguration configuration)
        {
            _momoAPI = momoAPI.Value;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnPost()
        {
            var userSessionInfo = HttpContext.Session.GetString(Resources.UserSessionInfo);
            if (string.IsNullOrEmpty(userSessionInfo))
            {
                return Redirect("/Login");
            }
            var userInfo = JsonConvert.DeserializeObject<UserInfoSession>(userSessionInfo);
            HoaHoeHoaSoi.Data.Models.Ordered ordered = null;

            using(var ctx = new HoaHoeHoaSoiContext())
            {
                ordered = ctx.Ordereds.FirstOrDefault(o => o.Id == OrderId);
            }

            if (ordered == null || ordered.PaymentMethod == (int)PaymentMethod.COD)
                return Redirect("/MyOrders/" + userInfo.Id);

            string payUrl = string.Empty;
            string paymentOrderId = string.Empty;

            if(ordered.PaymentMethod == (int)PaymentMethod.Momo)
            {
                var momoOrder = new MomoOrder
                {
                    CustomerName = userInfo.Name,
                    Total = Total,
                    OrderInfo = Resources.OrderInfoMessage
                };

                var response = await FuncHelpers.CreatePaymentAsync(momoOrder, _momoAPI);
                payUrl = response.PayUrl;
                paymentOrderId = response.OrderId;
            }
            else if(ordered.PaymentMethod == (int)PaymentMethod.VNPAY)
            {
                var vnpayOrder = new VnpayOrder
                {
                    CustomerName = userInfo.Name,
                    Total = Total,
                    OrderInfo = Resources.OrderInfoMessage,
                };
                var response = FuncHelpers.CreateVnPayPaymentAsync(vnpayOrder, _configuration, HttpContext);
                payUrl = response.PayUrl;
                paymentOrderId = response.OrderId;
            }
            
            if (string.IsNullOrEmpty(payUrl))
                return Redirect("/MyOrders/" + userInfo.Id);

            using (SqlConnection connection = HoaDBContext.GetSqlConnection())
            {
                connection.Open();
                string sql = "Update Ordered SET PaymentStatus = @PaymentStatus, PaymentOrderId = @PaymentOrderId WHERE Id = @OrderId";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@PaymentStatus", (int)PaymentStatus.UnPaid);
                    command.Parameters.AddWithValue("@PaymentOrderId", paymentOrderId);
                    command.Parameters.AddWithValue("@OrderId", OrderId);
                    command.ExecuteNonQuery();
                }
            }
            return Redirect(payUrl);
        }

        public void OnGet(string id)
        {
            Ordereds = new List<Ordered>();
            using (SqlConnection connection = HoaDBContext.GetSqlConnection())
            {
                connection.Open();

                string sql = "SELECT Id, Date, Total, PaymentStatus, PaymentNote, PaymentMethod, ResultCode From Ordered Where UserId = @UserId";
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
                            order.PaymentNote = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                            order.PaymentMethod = reader.IsDBNull(5) ? "COD" : ((PaymentMethod)reader.GetInt32(5)).ToString();
                            order.ResultCode = reader.IsDBNull(6) ? string.Empty : reader.GetString(6);
                            Ordereds.Add(order);
                        }
                    }
                }

                Ordereds = Ordereds.OrderByDescending(o => o.Id).ToList();

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
