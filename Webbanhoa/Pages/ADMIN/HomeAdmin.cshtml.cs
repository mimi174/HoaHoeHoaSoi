using HoaHoeHoaSoi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
namespace HoaHoeHoaSoi.Pages.ADMIN {
    public class HomeAdminModel : PageModel {
        
        public void OnGet() {
            string name = HttpContext.Session.GetString("Name");
            TempData["Name"] = name;
        }
    }
}



