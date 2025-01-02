namespace HoaHoeHoaSoi.Model
{
    public class Vnpay
    {
    }

    public class VnpayResponse
    {

    }

    public class VnpayOrder
    {
        public string Id { get; set; }
        public string CustomerName { get; set; }
        public double Total { get; set; }
        public string OrderInfo { get; set; }
    }

    public class VnpayAPI 
    { 
        public string Version { get; set; }
        public string Command { get; set; }
        public string TmnCode { get; set; }
        public string BankCode { get; set; }
        public string CurrCode { get; set; }
        public string IpAddr { get; set; }
        public string Locale { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class VnPayPaymentResponseModel
    {
        public string OrderDescription { get; set; }
        public string TransactionId { get; set; }
        public string OrderId { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentId { get; set; }
        public bool Success { get; set; }
        public string Token { get; set; }
        public string VnPayResponseCode { get; set; }
        public string Amount { get; set; }
    }

    public class VnPayCreatePaymentResponseModel
    {
        public string PayUrl { get; set; }
        public string OrderId { get; set; }
    }
}
