using HoaHoeHoaSoi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Webbanhoa.Pages;

public class IndexModel : PageModel {
    [BindProperty]
    public string name { get; set; }
    [BindProperty]
    public string phone { get; set; }
    [BindProperty]
    public string mail { get; set; }
    [BindProperty]
    public string Data { get; set; }
    
    private readonly ILogger<IndexModel> _logger;
    public bool hasData = false;
    public IndexModel(ILogger<IndexModel> logger) {
        _logger = logger;
    }

    public void OnGet() {

    }
    //TO-DO: Data Validation
    public void OnPost() {
        try {
            string connectionString =
                "Data Source=DESKTOP-0BD8UOC;Initial Catalog=HoaHoeHoaSoi;Persist Security Info=True;User ID=sa;Password=123456;Trust Server Certificate=True";
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();
                string sql = "INSERT INTO Customer" +
                             "(name,address,phone) VALUES" +
                             "(@name,@email,@phone);";
                string name = Request.Form["name"];
                string email = Request.Form["email"];
                string phone = Request.Form["phone"];
                
                using (SqlCommand command = new SqlCommand(sql, connection)) {
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@phone", phone);

                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception e) {
            _logger.LogError(e, "An error occurred while executing the query.");
        }
    }

}

