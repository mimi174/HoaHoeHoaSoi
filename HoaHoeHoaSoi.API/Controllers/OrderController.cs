using HoaHoeHoaSoi.API.Models;
using HoaHoeHoaSoi.API.ViewModels;
using HoaHoeHoaSoi.Data.Models;
using HoaHoeHoaSoi.Helpers;
using HoaHoeHoaSoi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using HoaHoeHoaSoiContext = HoaHoeHoaSoi.API.Models.HoaHoeHoaSoiContext;
using Ordered = HoaHoeHoaSoi.Model.Ordered;

namespace HoaHoeHoaSoi.API.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/order")]
    [ApiController]
    public class OrderController : BaseController
    {
        private MomoAPI _momoAPI { get; set; }
        public OrderController(HoaHoeHoaSoiContext dbContext, IOptions<MomoAPI> momoAPI) : base(dbContext)
        {
            _momoAPI = momoAPI.Value;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userInfo = GetUserFromToken();
            var orders = _dbContext.Ordereds.Where(o => o.UserId == userInfo.Id && o.PaymentStatus != (int)PaymentStatus.InCart).ToList();
            var result = new List<Ordered>();

            foreach(var order in orders)
            {
                var item = new Ordered
                {
                    Id = order.Id,
                    Date = order.Date,
                    PaymentStatus = (PaymentStatus)order.PaymentStatus,
                    Total = order.Total,
                };
                
                var orderLines = _dbContext.OrderLines.Where(ol => ol.OrderedId == order.Id).Include(ol => ol.Products).ToList();
                item.Lines = orderLines.Select(ol => new Model.OrderLine
                {
                    Id = ol.Id,
                    Price = ol.Price.Value,
                    ProductId = ol.ProductsId,
                    ProductName = ol.Products.Name,
                    Quantity = ol.Quantity.Value,     
                    Img = ol.Products.Img
                }).ToList();
                result.Add(item);
            }

            return Response(200, result);
        }

        [Authorize]
        [HttpPost("repay")]
        public async Task<IActionResult> RepayOrder(OrderRepayModel model)
        {
            var userInfo = GetUserFromToken();
            var order = _dbContext.Ordereds.FirstOrDefault(o => o.Id == model.Id && o.UserId == userInfo.Id);
            if (order == null)
                return Response(404, string.Empty, "Order not found");

            if (order.PaymentStatus == (int)PaymentStatus.Paid)
                return Response(400, string.Empty, "This order is already paid");

            var momoOrder = new MomoOrder
            {
                CustomerName = userInfo.Name,
                Total = order.Total.Value,
                OrderInfo = "Thanh toán tại HoaHoeHoaSoi"
            };
            var response = await FuncHelpers.CreatePaymentAsync(momoOrder, _momoAPI);

            order.PaymentStatus = (int)PaymentStatus.UnPaid;
            order.MomoOrderId = response.OrderId;
            _dbContext.Ordereds.Update(order);
            _dbContext.SaveChanges();

            return Response(200, new CheckoutResponse { OrderId = order.Id, PayURL = response.PayUrl });
        }
    }
}
