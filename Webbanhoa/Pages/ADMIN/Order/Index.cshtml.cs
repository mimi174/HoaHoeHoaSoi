using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HoaHoeHoaSoi.Helpers;
using HoaHoeHoaSoi.Model;
using Microsoft.IdentityModel.Tokens;
using HoaHoeHoaSoi.Data.Models;

namespace HoaHoeHoaSoi.Pages.ADMIN.Order {
    public class IndexModel : PageModel {
        [BindProperty]
        public string Search { get; set; }
        public List<OrderViewModel> OrderList { get; set; }

        public class OrderViewModel {
            public int OrderId { get; set; }
            public string CustomerName { get; set; }
            public string CustomerAddress { get; set; }
            public string CustomerPhone { get; set; }
            public DateTime Date { get; set; }
            public float Total { get; set; }
            public PaymentStatus PaymentStatus { get; set; }
        }
        public int DeleteId { get; set; }
        public void OnGet() {

            string name = HttpContext.Session.GetString("Name");
            TempData["Name"] = name;

            Search = string.Empty;
            var data = Request.Query["search"];
            if (!data.IsNullOrEmpty() && data.FirstOrDefault() != null)
                Search = data.FirstOrDefault();


            OrderList = new List<OrderViewModel>();
            
            using (var connection = HoaDBContext.GetSqlConnection()) {
                connection.Open();

                string command = $@"
                    SELECT
                        O.Id,
                        O.[ReceiverName],
                        O.[ReceiverAddress],
                        O.ReceiverPhone,
                        O.[Date],
                        O.Total,
                        O.PaymentStatus
                    FROM
                        Ordered AS O
                    WHERE O.Id LIKE N'%{Search}%' OR O.[ReceiverName] LIKE N'%{Search}%' OR
                        O.[ReceiverAddress] LIKE N'%{Search}%' OR O.ReceiverPhone LIKE N'%{Search}%' OR
                        CONVERT(NVARCHAR, O.[Date], 23) LIKE N'%{Search}%' OR 
                       LTRIM(STR(Total)) LIKE N'%{Search}%'
                ";


                using (var sqlCommand = new SqlCommand(command, connection)) {
                    using (var reader = sqlCommand.ExecuteReader()) {
                        while (reader.Read()) {

                            var order = new OrderViewModel {
                                OrderId = reader.GetInt32(0),
                                CustomerName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                CustomerAddress = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                CustomerPhone = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                Date = reader.GetDateTime(4),
                                Total = (float)reader.GetDouble(5),
                                PaymentStatus = (PaymentStatus) reader.GetInt32(6) 
                            };


                            OrderList.Add(order);
                        }
                    }
                }

                OrderList = OrderList.OrderByDescending(o => o.OrderId).ToList();
            }
        }

        public IActionResult OnPostDelete(int DeleteId) {
            if (DeleteId > 0) {
                using (var connection = HoaDBContext.GetSqlConnection()) {
                    connection.Open();


                    string deleteOrderLineCommand = $"DELETE FROM Order_Line WHERE Ordered_Id = {DeleteId}";
                    using (var sqlCommand = new SqlCommand(deleteOrderLineCommand, connection)) {
                        sqlCommand.ExecuteNonQuery();
                    }

                    string selectCustomerIdCommand = $"SELECT Customer_Id FROM Ordered WHERE Id = {DeleteId}";
                    int customerId = -1;
                    using (var selectCustomerIdCmd = new SqlCommand(selectCustomerIdCommand, connection)) {
                        using (var reader = selectCustomerIdCmd.ExecuteReader()) {
                            if (reader.Read()) {
                                customerId = reader.IsDBNull(0) ? -1 : reader.GetInt32(0);
                            }
                        }
                    }

                    string deleteOrderCommand = $"DELETE FROM Ordered WHERE Id = {DeleteId}";
                    using (var sqlCommand = new SqlCommand(deleteOrderCommand, connection)) {
                        sqlCommand.ExecuteNonQuery();
                    }

                    if (customerId > 0) {
                        string deleteCustomerCommand = $"DELETE FROM Customer WHERE Id = {customerId}";
                        using (var deleteCustomerCmd = new SqlCommand(deleteCustomerCommand, connection)) {
                            deleteCustomerCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            return RedirectToPage("Index");
        }
    }
}
