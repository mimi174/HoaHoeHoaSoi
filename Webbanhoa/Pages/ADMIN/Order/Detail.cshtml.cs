using System.Collections.Generic;
using System.Data.SqlClient;
using HoaHoeHoaSoi.Data.Models;
using HoaHoeHoaSoi.Helpers;
using HoaHoeHoaSoi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace HoaHoeHoaSoi.Pages.ADMIN.Order {
    public class DetailModel : PageModel {
        [BindProperty]
        public string Search { get; set; }
        public List<OrderLineViewModel> OrderLines { get; set; }

        public string OrderPaymentMethod { get; set; }
        public string OrderResultCode { get; set; }
        public double OrderTotal { get; set; }
        public string OrderPaymentNote { get; set; }
        public string OrderPaymentStatus { get; set; }
        public string OrderId { get; set; }
        [BindProperty]
        public int Id { get; set; }

        public class OrderLineViewModel {
            public int OrderLineId { get; set; }
            public string ProductName { get; set; }
            public double Price { get; set; }
            public int Quantity { get; set; }
        }
        public int DeleteId { get; set; }
        public IActionResult OnPostDelete(int deleteId) {
            int orderId = -1;
            string jsonStr = HttpContext.Session.GetString("OrderId");
            if (!string.IsNullOrEmpty(jsonStr)) {
                orderId = JsonConvert.DeserializeObject<int>(jsonStr);
            }

            if (deleteId > 0) {
                using (var connection = HoaDBContext.GetSqlConnection()) {
                    connection.Open();

                    string deleteOrderLineCommand = "DELETE FROM Order_Line WHERE Id = @OrderLineId";
                    using (var sqlCommand = new SqlCommand(deleteOrderLineCommand, connection)) {
                        sqlCommand.Parameters.AddWithValue("@OrderLineId", deleteId);
                        sqlCommand.ExecuteNonQuery();
                    }

                    string countOrderLineCommand = "SELECT COUNT(*) FROM Order_Line WHERE Ordered_Id = @OrderId";
                    using (var countCommand = new SqlCommand(countOrderLineCommand, connection)) {
                        countCommand.Parameters.AddWithValue("@OrderId", orderId);
                        int orderLineCount = (int)countCommand.ExecuteScalar();


                        if (orderLineCount == 0) {
                            int customerId = -1;
                            string selectCustomerIdCommand = "SELECT Customer_Id FROM Ordered WHERE Id = @OrderId";
                            using (var selectCustomerIdCmd = new SqlCommand(selectCustomerIdCommand, connection)) {
                                selectCustomerIdCmd.Parameters.AddWithValue("@OrderId", orderId);
                                customerId = (int)selectCustomerIdCmd.ExecuteScalar();
                            }

                            string deleteOrderedCommand = "DELETE FROM Ordered WHERE Id = @OrderId";
                            using (var deleteOrderedCmd = new SqlCommand(deleteOrderedCommand, connection)) {
                                deleteOrderedCmd.Parameters.AddWithValue("@OrderId", orderId);
                                deleteOrderedCmd.ExecuteNonQuery();
                            }

                            string deleteCustomerCommand = "DELETE FROM Customer WHERE Id = @CustomerId";
                            using (var deleteCustomerCmd = new SqlCommand(deleteCustomerCommand, connection)) {
                                deleteCustomerCmd.Parameters.AddWithValue("@CustomerId", customerId);
                                deleteCustomerCmd.ExecuteNonQuery();
                            }

                        }
                    }
                }
            }
            return RedirectToPage("/ADMIN/Order/Detail", new { id = orderId });
        }

        public IActionResult OnPost()
        {
            using(var ctx = new HoaHoeHoaSoiContext())
            {
                var order = ctx.Ordereds.FirstOrDefault(o => o.Id == Id);
                if(order != null && order.PaymentMethod == (int)PaymentMethod.COD 
                    && order.PaymentStatus != (int)PaymentStatus.InCart && order.PaymentStatus != (int)PaymentStatus.Paid)
                {
                    order.PaymentStatus = (int)PaymentStatus.Paid;
                    ctx.SaveChanges();
                }
            }
            return Redirect("/ADMIN/Order/Detail/" + Id);
        }

        public IActionResult OnGet(int id) {
            Id = id;
            string jsonStr = JsonConvert.SerializeObject(id);
            HttpContext.Session.SetString("OrderId", jsonStr);

            string name = HttpContext.Session.GetString("Name");
            TempData["Name"] = name;

            Search = string.Empty;
            var data = Request.Query["search"];
            if (!data.IsNullOrEmpty() && data.FirstOrDefault() != null)
                Search = data.FirstOrDefault();


            if (id <= 0) {
                return RedirectToPage("/Error");
            }

            OrderLines = new List<OrderLineViewModel>();

            using(var ctx = new HoaHoeHoaSoiContext())
            {
                var order = ctx.Ordereds.FirstOrDefault(o => o.Id == id);
                if (order != null) {
                    if (order.PaymentMethod == null)
                        order.PaymentMethod = (int)PaymentMethod.COD;

                    OrderPaymentMethod = ((PaymentMethod) order.PaymentMethod).ToString();
                    OrderResultCode = order.ResultCode;
                    OrderPaymentNote = order.PaymentNote;
                    OrderTotal = order.Total.Value;
                    OrderId = order.PaymentOrderId;
                    OrderPaymentStatus = ((PaymentStatus)order.PaymentStatus).ToString();
                }
            }

            using (var connection = HoaDBContext.GetSqlConnection()) {
                connection.Open();

                string query = $@"
                SELECT
                    OL.Id,
                    P.[Name],
                    OL.Price,
                    OL.Quantity
                FROM
                    Order_Line AS OL
                INNER JOIN
                    Products AS P ON OL.Products_Id = P.Id
                WHERE
                    OL.Ordered_Id = @OrderId AND
                    (OL.Id LIKE '%{Search}%' OR 
                     P.[Name] LIKE '%{Search}%' OR 
                     CONVERT(VARCHAR(20), OL.Price) LIKE '%{Search}%' OR 
                     CONVERT(VARCHAR(20), OL.Quantity) LIKE '%{Search}%')
            ";



                using (var sqlCommand = new SqlCommand(query, connection)) {
                    sqlCommand.Parameters.AddWithValue("@OrderId", id);

                    using (var reader = sqlCommand.ExecuteReader()) {
                        while (reader.Read()) {

                            var orderLine = new OrderLineViewModel {
                                OrderLineId = reader.GetInt32(0),
                                ProductName = reader.GetString(1),
                                Price = reader.GetDouble(2),
                                Quantity = reader.GetInt32(3)
                            };

                            OrderLines.Add(orderLine);

                        }
                    }
                }
            }
            return Page();
        }
    }
}
