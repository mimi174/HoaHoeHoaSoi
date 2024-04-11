using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HoaHoeHoaSoi.Helpers;
using HoaHoeHoaSoi.Model;
using Microsoft.IdentityModel.Tokens;

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
                        C.[Name],
                        C.[Address],
                        C.Phone,
                        O.[Date],
                        O.Total
                    FROM
                        Ordered AS O
                    INNER JOIN
                        Customer AS C ON O.Customer_Id = C.Id
                    WHERE O.Id LIKE '%{Search}%' OR C.[Name] LIKE '%{Search}%' OR
                        C.[Address] LIKE '%{Search}%' OR C.Phone LIKE '%{Search}%' OR
                        CONVERT(NVARCHAR, O.[Date], 23) LIKE '%{Search}%' OR 
                        CAST(O.Total AS NVARCHAR) LIKE '%{Search}%'
                ";


                using (var sqlCommand = new SqlCommand(command, connection)) {
                    using (var reader = sqlCommand.ExecuteReader()) {
                        while (reader.Read()) {

                            var order = new OrderViewModel {
                                OrderId = reader.GetInt32(0),
                                CustomerName = reader.GetString(1),
                                CustomerAddress = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                CustomerPhone = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                Date = reader.GetDateTime(4),
                                Total = (float)reader.GetDouble(5)
                            };


                            OrderList.Add(order);
                        }
                    }
                }
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
                                customerId = reader.GetInt32(0);
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
