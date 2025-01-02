using HoaHoeHoaSoi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace HoaHoeHoaSoi.Pages.ADMIN.Product {
    public class ProductPageModel : PageModel {
        [BindProperty]
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Name { get; set; }
        [BindProperty]
        [Required]
        [Range(50000, 5000000, ErrorMessage = "The price must between 50,000 and 5,000,000")]
        public double Price { get; set; }
        [BindProperty]
        [Required]
        [ImageExtenstion]
        public IFormFile Img { get; set; }

        [BindProperty]
        public string Description { get; set; }

        protected string ConvertImgToBase64() {
            if (Img == null)
                return string.Empty;

            string imgValue = string.Empty;
            using (var ms = new MemoryStream()) {
                Img.CopyTo(ms);
                imgValue = Convert.ToBase64String(ms.ToArray());
            }

            return imgValue;
        }
    }
}
