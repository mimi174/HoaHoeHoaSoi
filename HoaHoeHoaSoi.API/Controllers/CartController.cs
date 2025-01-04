using Azure;
using HoaHoeHoaSoi.API.Models;
using HoaHoeHoaSoi.API.ViewModels;
using HoaHoeHoaSoi.Data.Models;
using HoaHoeHoaSoi.Helpers;
using HoaHoeHoaSoi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net;
using System.Numerics;
using System.Resources;
using HoaHoeHoaSoiContext = HoaHoeHoaSoi.API.Models.HoaHoeHoaSoiContext;

namespace HoaHoeHoaSoi.API.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : BaseController
    {
        private MomoAPI _momoAPI;
        private readonly IConfiguration _configuration;
        private int _paymentStatus = (int)PaymentStatus.UnPaid;
        private PaymentExecuteResponseModel _response;
        public CartController(HoaHoeHoaSoiContext dbContext, IOptions<MomoAPI> momoAPI, IConfiguration configuration) : base(dbContext)
        {
            _momoAPI = momoAPI.Value;
            _configuration = configuration;
        }

        [Authorize]
        [HttpPost("add-to-cart")]
        public IActionResult AddToCart([FromBody] AddToCartModel model)
        {
            var user = GetUserFromToken();
            var product = _dbContext.Products.FirstOrDefault(p => p.Id == model.ProudctId);
            if (product == null)
                return Response(404, string.Empty, "Product not found");

            CartHelper.AddToCard(model.ProudctId, model.Quantity, user.Id, product.Price.Value);
            return Response(200, true);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            var user = GetUserFromToken();
            var products = CartHelper.LoadCartInfo(user.Id);
            var result = new CartViewModel
            {
                Products = products,
                Total = products.Sum(p => p.Quantity * p.Price)
            };
            return Response(200, result);
        }

        [Authorize]
        [HttpPut("update-cart")]
        public IActionResult UpdateCart([FromBody] List<AddToCartModel> model)
        {
            var user = GetUserFromToken();
            CartHelper.UpdateCart(model.ToDictionary(p => p.ProudctId, p => p.Quantity), user.Id);
            return Response(200, true);
        }

        [Authorize]
        [HttpDelete("remove-product/{productId}")]
        public IActionResult RemoveProductFromCart(int productId)
        {
            var user = GetUserFromToken();
            CartHelper.RemoveProductFromCart(productId, user.Id);
            return Response(200, true);
        }

        [Authorize]
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout(CheckoutModel model)
        {
            var userInfo = GetUserFromToken();
            string paymentOrderId = string.Empty;
            string payURL = string.Empty;

            var order = _dbContext.Ordereds.FirstOrDefault(o => o.UserId == userInfo.Id && o.PaymentStatus == (int)PaymentStatus.InCart);

            if (order == null) {
                return Response(400, string.Empty, "Cart not found");
            }

            if (model.PaymentMethod == PaymentMethod.Momo)
            {
                order.Total = Math.Ceiling(order.Total.Value * 0.8);
                order.PaymentMethod = (int)PaymentMethod.Momo;                

                var momoOrder = new MomoOrder
                {
                    CustomerName = userInfo.Name,
                    Total = order.Total.Value,
                    OrderInfo = "Thanh toán tại HoaHoeHoaSoi"
                };
                var response = await FuncHelpers.CreatePaymentAsync(momoOrder, _momoAPI);
                payURL = response.PayUrl;
                paymentOrderId = response.OrderId;
            }
            else if (model.PaymentMethod == PaymentMethod.VNPAY)
            {
                order.Total = Math.Ceiling(order.Total.Value * 0.8);
                order.PaymentMethod = (int)PaymentMethod.VNPAY;

                var vnpayOrder = new VnpayOrder
                {
                    CustomerName = userInfo.Name,
                    Total = order.Total.Value,
                    OrderInfo = "Thanh toán tại HoaHoeHoaSoi",
                };
                var response = FuncHelpers.CreateVnPayPaymentAsync(vnpayOrder, _configuration, HttpContext);
                payURL = response.PayUrl;
                paymentOrderId = response.OrderId;
            }

            _dbContext.Ordereds.Update(order);
            _dbContext.SaveChanges();

            CartHelper.ProcessCartIntoOrder(userInfo.Id, model.ReceiverName, model.ReceiverAddress, model.ReceiverPhone, paymentOrderId);
            return Response(200, new CheckoutResponse { OrderId = order.Id, PayURL = payURL});
        }

        [HttpGet("check-payment-status")]
        public IActionResult CheckPaymentStatus()
        {
            if (HttpContext.Request.Query.Count == 0)
                return Response(404, string.Empty, "Payment not found");

            if (HttpContext.Request.Query.Any(k => k.Key.StartsWith("vnp_")))
            {
                HandleVNPayResponse(HttpContext.Request.Query);
            }
            else if (HttpContext.Request.Query.Any(k => k.Key.Contains("orderInfo")))
            {
                HandleMomoResponse(HttpContext.Request.Query);
            }

            var order = _dbContext.Ordereds.FirstOrDefault(o => o.PaymentOrderId == _response.OrderId);
            if (order != null)
            {
                order.PaymentNote = _response.LocalMessage;
                order.ResultCode = _response.ErrorCode;
                order.PaymentStatus = _paymentStatus;

                _dbContext.SaveChanges();
            }

            //if (HttpContext.Request.Query.Count == 0)
            //    return Response(404, string.Empty, "Payment not found");
            //var response = FuncHelpers.PaymentExecuteAsync(HttpContext.Request.Query);
            //int paymentStatus = response.ErrorCode == "0" ? (int)PaymentStatus.Paid : (int)PaymentStatus.Failed;

            //var order = _dbContext.Ordereds.FirstOrDefault(o => o.PaymentOrderId == response.OrderId);
            //if (order == null)
            //    return Response(404, string.Empty, "Order not found");

            //order.PaymentStatus = paymentStatus;
            //_dbContext.Ordereds.Update(order);
            //_dbContext.SaveChanges();

            return Response(200, _response);
        }

        private void HandleVNPayResponse(IQueryCollection query)
        {
            var pay = new VnPayLibrary();
            var vnPayResponse = pay.GetFullResponseData(query, _configuration["Vnpay:HashSecret"]);

            _response = new PaymentExecuteResponseModel
            {
                Amount = vnPayResponse.Amount,
                OrderInfo = vnPayResponse.OrderDescription,
                OrderId = vnPayResponse.OrderId,
                ErrorCode = vnPayResponse.VnPayResponseCode,
                LocalMessage = vnPayResponse.VnPayResponseCode != null ? Properties.Resources.ResourceManager.GetString(vnPayResponse.VnPayResponseCode) : string.Empty,
                Success = vnPayResponse.Success
            };

            _paymentStatus = _response.Success ? (int)PaymentStatus.Paid : (int)PaymentStatus.Failed;
        }

        private void HandleMomoResponse(IQueryCollection query)
        {
            _response = FuncHelpers.PaymentExecuteAsync(HttpContext.Request.Query);
            _paymentStatus = _response.ErrorCode == "0" ? (int)PaymentStatus.Paid : (int)PaymentStatus.Failed;
        }
    }
}
