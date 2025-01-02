using Newtonsoft.Json;

namespace HoaHoeHoaSoi.Model
{
    public class Ordered
    {
        public int Id { get; set; }
        public DateOnly? Date { get; set; }
        public double? Total { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string PaymentNote { get; set; }
        public string PaymentMethod { get; set; }
        public string ResultCode { get; set; }
        [JsonProperty("Lines")]
        public List<OrderLine> Lines { get; set; } = new List<OrderLine>();
    }

    public class OrderLine
    {
        public int Id { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Img { get; set; }
    }

    public enum PaymentStatus
    {
        UnPaid = 0,
        Paid,
        Failed,
        InCart
    }

    public enum PaymentMethod
    {
        Momo = 0,
        COD,
        VNPAY
    }
}
