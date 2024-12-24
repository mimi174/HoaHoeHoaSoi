using HoaHoeHoaSoi.Model;

namespace HoaHoeHoaSoi.API.ViewModels
{
    public class CartVMs
    {
    }

    public class AddToCartModel
    {
        public int ProudctId { get; set; }
        public int Quantity { get; set; }
    }

    public class CheckoutModel
    {
        public PaymentMethod PaymentMethod { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverAddress { get; set; }
        public string ReceiverPhone { get; set; }
    }

    public class CheckoutResponse
    {
        public string PayURL { get; set; }
        public int OrderId { get; set; }
    }

    public class OrderRepayModel
    {
        public int Id { get; set; }
    }

    public class CartViewModel
    {
        public double Total { get; set; }
        public List<Products> Products { get; set; } = new List<Products>();
    }
}
