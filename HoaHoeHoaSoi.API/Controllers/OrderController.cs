using HoaHoeHoaSoi.API.Models;
using HoaHoeHoaSoi.API.ViewModels;
using HoaHoeHoaSoi.Data.Models;
using HoaHoeHoaSoi.Helpers;
using HoaHoeHoaSoi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private IConfiguration _configuration { get; set; }
        public OrderController(HoaHoeHoaSoiContext dbContext, IOptions<MomoAPI> momoAPI, IConfiguration configuration) : base(dbContext)
        {
            _momoAPI = momoAPI.Value;
            _configuration = configuration;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userInfo = GetUserFromToken();
            var orders = _dbContext.Ordereds
                .Where(o => o.UserId == userInfo.Id && o.PaymentStatus != (int)PaymentStatus.InCart)
                .Select(o => new
                {
                    o.Id,
                    o.Date,
                    PaymentStatus = (PaymentStatus)o.PaymentStatus,
                    o.Total
                })
                .ToList();

            return Response(200, orders);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            var userInfo = GetUserFromToken();
            var order = _dbContext.Ordereds
                .Where(o => o.Id == id && o.UserId == userInfo.Id)
                .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Products)
                .FirstOrDefault();
            if (order == null)
                return Response(404, string.Empty, "Order not found");
            var orderDetails = new
            {
                order.Id,
                order.Date,
                PaymentStatus = (PaymentStatus)order.PaymentStatus,
                order.Total,
                Lines = order.OrderLines.Select(ol => new
                {
                    ol.Id,
                    productId = ol.ProductsId,
                    Name = ol.Products.Name,
                    Quantity = ol.Quantity,
                    Price = ol.Price,
                    Img = ol.Products.Img
                }).ToList()
            };

            return Response(200, orderDetails);
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

            string payUrl = string.Empty;
            if(order.PaymentMethod == (int)PaymentMethod.Momo)
            {
                var momoOrder = new MomoOrder
                {
                    CustomerName = userInfo.Name,
                    Total = order.Total.Value,
                    OrderInfo = "Thanh toán tại HoaHoeHoaSoi"
                };
                var response = await FuncHelpers.CreatePaymentAsync(momoOrder, _momoAPI);
                order.PaymentOrderId = response.OrderId;
                payUrl = response.PayUrl;
            }
            else if(order.PaymentMethod == (int)PaymentMethod.VNPAY)
            {
                var vnpayOrder = new VnpayOrder
                {
                    CustomerName = userInfo.Name,
                    Total = order.Total.Value,
                    OrderInfo = "Thanh toán tại HoaHoeHoaSoi",
                };
                var response = FuncHelpers.CreateVnPayPaymentAsync(vnpayOrder, _configuration, HttpContext);
                payUrl = response.PayUrl;
                order.PaymentOrderId = response.OrderId;
            }

            order.PaymentStatus = (int)PaymentStatus.UnPaid;
            order.PaymentNote = string.Empty;
            order.ResultCode = string.Empty;
            _dbContext.Ordereds.Update(order);
            _dbContext.SaveChanges();

            return Response(200, new CheckoutResponse { OrderId = order.Id, PayURL = payUrl });
        }
    }
}