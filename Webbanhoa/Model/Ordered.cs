namespace HoaHoeHoaSoi.Model
{
    public class Ordered
    {
        public int Id { get; set; }
        public DateOnly? Date { get; set; }
        public double? Total { get; set; }
        public PaymentStatus PaymentStatus { get; set; }

        public List<OrderLine> Lines = new List<OrderLine> { };
    }

    public class OrderLine
    {
        public int Id { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }

    public enum PaymentStatus
    {
        UnPaid = 0,
        Paid,
        Failed,
        InCart
    }
}
