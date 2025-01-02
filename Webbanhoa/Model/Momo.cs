using Newtonsoft.Json;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HoaHoeHoaSoi.Model
{
    public class Momo
    {
    }

    public class MomoAPI
    {
        public string MomoApiUrl { get; set; }
        public string SecretKey { get; set; }
        public string AccessKey { get; set; }
        public string ReturnUrl { get; set; }
        public string NotifyUrl { get; set; }
        public string PartnerCode { get; set; }
        public string RequestType { get; set; }
    }

    public class MomoOrder
    {
        public string Id { get; set; }
        public string CustomerName { get; set; }
        public DateOnly? Date { get; set; }
        public double Total { get; set; }
        public string OrderInfo { get; set; }
    }

    public class MomoResponse
    {
        public string RequestId { get; set; }
        public int ErrorCode { get; set; }
        public string OrderId { get; set; }
        public string Message { get; set; }
        public string LocalMessage { get; set; }
        public string RequestType { get; set; }
        public string PayUrl { get; set; }
        public string Signature { get; set; }
        public string QrCodeUrl { get; set; }
        public string Deeplink { get; set; }
        public string DeeplinkWebInApp { get; set; }
    }

    public class PaymentExecuteResponseModel
    {
        public string OrderId { get; set; }
        public string Amount { get; set; }
        public string OrderInfo { get; set; }
        public string LocalMessage { get; set; }
        public string ErrorCode { get; set; }
        public bool Success { get; set; }
    }
}
