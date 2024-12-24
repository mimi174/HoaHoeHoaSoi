﻿using HoaHoeHoaSoi.API.Models;
using HoaHoeHoaSoi.API.ViewModels;
using HoaHoeHoaSoi.Model;
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
                products = products.Include(p => p.OrderLines).OrderByDescending(p => p.OrderLines.Sum(ol => ol.Quantity));
            }

            var total = products.Count();
            products = products.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize);

            return Response(200, products.Select(p => new ProductViewModel { Id = p.Id, Description = p.Description, Name = p.Name, Price = p.Price }).ToList(), total, filter.Page, products.Count());
        }
    }
}
