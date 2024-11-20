using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using HoaHoeHoaSoi.Helpers;
using HoaHoeHoaSoi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HoaHoeHoaSoi.Pages
{
	    public class UserInfoModel : PageModel
    {
        public string Id { get; set; }
        public UserInfo userInfo = new UserInfo();
        public string errorMessage = "";
        public void OnGet(string id)
        {
            //string Id = Request.Query["Id"];
            Id = id;
            try
            {
                using (SqlConnection connection = HoaDBContext.GetSqlConnection())
                {
                    connection.Open();
                    string sql = "SELECT * FROM UserInfo where Id=@Id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", Id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())   
                            {          
                                userInfo.Id = reader.GetInt32(0);
                                userInfo.Username = reader.GetString(1);
                                userInfo.Password = reader.GetString(2);
                                userInfo.Name = reader.GetString(3);
                                userInfo.DOB = reader.IsDBNull(4) ? null : reader.GetDateTime(4);
                                userInfo.Address = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                                userInfo.Phone = reader.IsDBNull(6) ? string.Empty : reader.GetString(6);
                                userInfo.Avatar = reader.IsDBNull(7) ? string.Empty : reader.GetString(7);
                                userInfo.Gender = reader.IsDBNull(8) ? false : reader.GetBoolean(8);
                                userInfo.Mail = reader.IsDBNull(9) ? string.Empty : reader.GetString(9);
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

        public void OnPost()
        {
            //userInfo.Id = Request.Form["Id"];
            //userInfo.Username = Request.Form["Username"];
            //userInfo.Password = Request.Form["Password"];
            //userInfo.Name = Request.Form["Name"];
            //userInfo.DOB = Request.Form["DOB"];
            //userInfo.Address = Request.Form["Address"];
            //userInfo.Phone = Request.Form["Phone"];
            //userInfo.Avatar = Request.Form["Avatar"];
            //userInfo.Gender = Request.Form["Gender"];
            //userInfo.Mail = Request.Form["Mail"];


            ////check all fields are filled 
            //if (string.IsNullOrEmpty(userInfo.email) || string.IsNullOrEmpty(userInfo.pass) || string.IsNullOrEmpty(userInfo.fName) || string.IsNullOrEmpty(userInfo.lName)
            //    || string.IsNullOrEmpty(userInfo.gender) || string.IsNullOrEmpty(userInfo.birthDay) || string.IsNullOrEmpty(userInfo.phone) || string.IsNullOrEmpty(userInfo.address))
            //{
            //    errorMessage = "All fields are required!!!";
            //    return;
            //}

            //try
            //{
            //    string connectionString = "Data Source=THANHHOA\\MSSQLSERVER01;Initial Catalog=HFINANCE;Integrated Security = True; Pooling = False; TrustServerCertificate = True";
            //    using (SqlConnection connection = new SqlConnection(connectionString))
            //    {
            //        connection.Open();

            //        string sql = "UPDATE Users SET email=@email,passWord=@pass,firstName=@fName,lastName=@lName,gender=@gender,birthDay=@birthDay,address=@address,phone=@phone WHERE userId=@id";
            //        using (SqlCommand command = new SqlCommand(sql, connection))
            //        {
            //            command.Parameters.AddWithValue("@email", userInfo.email);
            //            command.Parameters.AddWithValue("@pass", userInfo.pass);
            //            command.Parameters.AddWithValue("@fName", userInfo.fName);
            //            command.Parameters.AddWithValue("@lName", userInfo.lName);
            //            command.Parameters.AddWithValue("@gender", userInfo.gender);
            //            command.Parameters.AddWithValue("@birthDay", userInfo.birthDay);
            //            command.Parameters.AddWithValue("@address", userInfo.address);
            //            command.Parameters.AddWithValue("@phone", userInfo.phone);
            //            command.Parameters.AddWithValue("@id", userInfo.id);

            //            command.ExecuteNonQuery();
            //        }
            //    }

            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //    throw;
            //}
            //Response.Redirect("/Users/Infor?id=" + userInfo.id);
        }
    }
}

