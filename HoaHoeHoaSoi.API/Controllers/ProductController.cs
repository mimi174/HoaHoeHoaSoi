using HoaHoeHoaSoi.API.Models;
using HoaHoeHoaSoi.API.ViewModels;
using HoaHoeHoaSoi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using ProductViewModel = HoaHoeHoaSoi.API.ViewModels.ProductViewModel;

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
            if (filter.SortByBestSeller)
            {
                //var data = _dbContext.OrderLines
                //    .Where(ol => products.Select(p => ol.ProductsId).Contains(ol.ProductsId) && ol.Ordered.PaymentStatus != (int)PaymentStatus.InCart)
                //    .GroupBy(ol => ol.ProductsId)
                //    .Select(ol => new { ProductId = ol.Key, Seller = ol.Sum(o => o.Quantity) })
                //    .OrderByDescending(ol => ol.Seller).ToList();

                //var sortedIds = data.Select(d => d.ProductId).ToList();
                //products = products.ToList().OrderBy(p => sortedIds.IndexOf(p.Id)).AsQueryable();
                products = products.Include(p => p.OrderLines)
                    .OrderByDescending(p => p.OrderLines.AsQueryable().Include(ol => ol.Ordered).Where(ol => ol.Ordered.PaymentStatus != (int)PaymentStatus.InCart).Sum(ol => ol.Quantity));
            }

            var total = products.Count();
            products = products.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize);

            return Response(200, products.Select(p => new ProductViewModel { Id = p.Id, Description = p.Description, Name = p.Name, Price = p.Price, Img = p.Img }).ToList(), total, filter.Page, products.Count());
        }

        [HttpPost("add-to-wishlist")]
        [Authorize]
        public IActionResult AddToWishList(ProductWishListCreateModel model)
        {
            var user = GetUserFromToken();
            var product = _dbContext.Products.FirstOrDefault(p => p.Id == model.ProductId);
            if (product == null)
                return Response(404, string.Empty, "Product not found");

            var productWishList = _dbContext.ProductWishlists.FirstOrDefault(pw => pw.UserId == user.Id && pw.ProductId == model.ProductId);
            if (productWishList == null) 
            {
                productWishList = new ProductWishlist
                {
                    ProductId = model.ProductId,
                    UserId = user.Id,
                };

                _dbContext.ProductWishlists.Add(productWishList);
                _dbContext.SaveChanges();
            }

            return Response(200, string.Empty);
        }

        [HttpDelete("remove-from-wishlist/{id}")]
        [Authorize]
        public IActionResult RemoveFromWishList(int id)
        {
            var user = GetUserFromToken();
            var productWishList = _dbContext.ProductWishlists.FirstOrDefault(pw => pw.UserId == user.Id && pw.ProductId == id);
            if (productWishList != null)
            {
                _dbContext.ProductWishlists.Remove(productWishList);
                _dbContext.SaveChanges();
            }

            return Response(200, string.Empty);
        }

        [HttpGet("wishlist")]
        [Authorize]
        public IActionResult GetWishList([FromQuery] ProductWishListFilter filter)
        {
            var user = GetUserFromToken();
            var productIds = _dbContext.ProductWishlists.Where(pw => pw.UserId == user.Id).Select(p => p.ProductId).ToList();
            var products = _dbContext.Products.Where(p => productIds.Contains(p.Id));

            if (filter.ProductId != 0)
                products = products.Where(p => p.Id == filter.ProductId);
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
