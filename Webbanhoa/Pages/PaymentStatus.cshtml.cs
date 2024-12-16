using HoaHoeHoaSoi.Helpers;
using HoaHoeHoaSoi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Net;
using System.Numerics;
using System.Reflection;

namespace HoaHoeHoaSoi.Pages
{
    public class PaymentStatusModel : PageModel
    {
        public MomoExecuteResponseModel Response { get; set; }
        public void OnGet()
        {
            if (HttpContext.Request.Query.Count == 0)
                return;
            Response = FuncHelpers.PaymentExecuteAsync(HttpContext.Request.Query);
            int paymentStatus = Response.ErrorCode == "0" ? (int)PaymentStatus.Paid : (int)PaymentStatus.Failed;

            using (SqlConnection connection = HoaDBContext.GetSqlConnection())
            {
                connection.Open();
                string sql = "Update Ordered SET PaymentStatus = @PaymentStatus WHERE MomoOrderId = @MomoOrderId";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@PaymentStatus", paymentStatus);
                    command.Parameters.AddWithValue("@MomoOrderId", Response.OrderId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
