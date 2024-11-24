using HoaHoeHoaSoi.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HoaHoeHoaSoi.Pages {
    public class LogoutModel : PageModel {
        public IActionResult OnGet() {
            HttpContext.Session.Clear();
            return RedirectToPage("/ADMIN");
        }

        public IActionResult OnPost()
        {
            HttpContext.Session.Clear();
            return Redirect("/");
        }
    }
}
