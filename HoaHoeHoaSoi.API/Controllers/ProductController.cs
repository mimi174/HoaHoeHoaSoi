using HoaHoeHoaSoi.API.Models;
using HoaHoeHoaSoi.API.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HoaHoeHoaSoi.API.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : BaseController
    {
        public ProductController(HoaHoeHoaSoiContext dbContext) : base(dbContext)
        {
        }

        [HttpGet]
        public IActionResult Get([FromQuery]ProductFilterModel filter)
        {
            var products = _dbContext.Products.AsQueryable();
            if(filter.Id != 0)
                products = products.Where(p => p.Id == filter.Id);
            if (!string.IsNullOrEmpty(filter.Search))
            {
                var search = filter.Search.ToLower();
                products = products.Where(p =>
                    p.Name != null && p.Name.ToLower().Contains(search)
                    || p.Description != null && p.Description.ToLower().Contains(search)
                );
            }
            var total = products.Count();
            products = products.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize);

            return Response(200, products.Select(p => new ProductViewModel { Id = p.Id, Description = p.Description, Name = p.Name, Price = p.Price, Img = p.Img }).ToList(), total, filter.Page, products.Count());
        }
    }
}
