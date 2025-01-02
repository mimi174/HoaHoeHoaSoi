using HoaHoeHoaSoi.Data.Models;
using HoaHoeHoaSoi.Helpers;
using HoaHoeHoaSoi.Model;
using HoaHoeHoaSoi.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data.SqlClient;
using System.Net;
using System.Numerics;
using System.Reflection;

namespace HoaHoeHoaSoi.Pages
{
    public class PaymentStatusModel : PageModel
    {
        public MomoExecuteResponseModel Response { get; set; }
        private readonly IConfiguration _configuration;
        int _paymentStatus = (int)PaymentStatus.UnPaid;
        public PaymentStatusModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            if (HttpContext.Request.Query.Count == 0)
                return;

            if (HttpContext.Request.Query.Any(k => k.Key.StartsWith("vnp_")))
            {
                HandleVNPayResponse(HttpContext.Request.Query);
            }
            else if (HttpContext.Request.Query.Any(k => k.Key.Contains("orderInfo")))
            {
                HandleMomoResponse(HttpContext.Request.Query);
            }

            using (var ctx = new HoaHoeHoaSoiContext())
            {
                var order = ctx.Ordereds.FirstOrDefault(o => o.PaymentOrderId == Response.OrderId);
                if (order != null)
                {
                    order.PaymentNote = Response.LocalMessage;
                    order.ResultCode = Response.ErrorCode;
                    order.PaymentStatus = _paymentStatus;

                    ctx.SaveChanges();
                }
            }
        }

        private void HandleVNPayResponse(IQueryCollection query)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(query, _configuration["Vnpay:HashSecret"]);
            Response = new MomoExecuteResponseModel
            {
                Amount = response.Amount,
                OrderInfo = response.OrderDescription,
                OrderId = response.OrderId,
                ErrorCode = response.VnPayResponseCode,
                LocalMessage = response.VnPayResponseCode != null ? Properties.Resources.ResourceManager.GetString(response.VnPayResponseCode) : string.Empty
            };

            _paymentStatus = response.Success ? (int)PaymentStatus.Paid : (int)PaymentStatus.Failed;
        }

        private void HandleMomoResponse(IQueryCollection query)
        {
            Response = FuncHelpers.PaymentExecuteAsync(HttpContext.Request.Query);
            _paymentStatus = Response.ErrorCode == "0" ? (int)PaymentStatus.Paid : (int)PaymentStatus.Failed;
        }
    }
}
