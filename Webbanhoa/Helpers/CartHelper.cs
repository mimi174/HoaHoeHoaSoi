using HoaHoeHoaSoi.Data.Models;
using HoaHoeHoaSoi.Model;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace HoaHoeHoaSoi.Helpers
{
    public static class CartHelper
    {
        public static void AddToCard(int productId, int quantity, int userId, double price)
        {
            int orderId = 0;

            using (var ctx = new HoaHoeHoaSoiContext())
            {
                var order = ctx.Ordereds.FirstOrDefault(o => o.UserId == userId && o.PaymentStatus == (int)PaymentStatus.InCart);
                if (order == null)
                {
                    order = new Data.Models.Ordered
                    {
                        Date = DateTime.Now,
                        UserId = userId,
                        PaymentStatus = (int)PaymentStatus.InCart,
                    };
                    ctx.Ordereds.Add(order);
                    ctx.SaveChanges();
                }
                orderId = order.Id;
                var orderLine = ctx.OrderLines.FirstOrDefault(ol => ol.OrderedId == order.Id && ol.ProductsId == productId);
                if (orderLine == null) 
                {
                    orderLine = new Data.Models.OrderLine
                    {
                        OrderedId = order.Id,
                        ProductsId = productId,
                        Quantity = quantity,
                        Price = price
                    };
                    ctx.OrderLines.Add(orderLine);
                    ctx.SaveChanges();
                }
            }

            UpdateOrderTotal(orderId);
        }

        public static void UpdateOrderTotal(int orderId)
        {
            using (var ctx = new HoaHoeHoaSoiContext())
            {
                var orderLines = ctx.OrderLines.Where(ol => ol.OrderedId == orderId).ToList();
                if (orderLines.Count > 0)
                {
                    var total = orderLines.Sum(ol => ol.Price * ol.Quantity);

                    var order = ctx.Ordereds.FirstOrDefault(o => o.Id == orderId);
                    order.Total = total;
                    ctx.SaveChanges();
                }
            }
        }

        public static HoaHoeHoaSoi.Data.Models.Ordered GetCartByUserId(int userId)
        {
            using(var ctx = new HoaHoeHoaSoiContext())
            {
                return ctx.Ordereds.FirstOrDefault(o => o.UserId == userId && o.PaymentStatus == (int)PaymentStatus.InCart);
            }
        }

        public static List<Products> LoadCartInfo(int userId)
        {
            using (var ctx = new HoaHoeHoaSoiContext())
            {
                var order = GetCartByUserId(userId);

                if (order == null)
                    return new List<Products>();

                return ctx.OrderLines.Where(ol => ol.OrderedId == order.Id).Include(ol => ol.Products).Select(ol => new Products
                {
                    Id = ol.ProductsId,
                    Img = ol.Products.Img,
                    Name = ol.Products.Name,
                    Price = ol.Price.Value,
                    Quantity = ol.Quantity.Value
                }).ToList();
            }
        }

        public static void UpdateCart(int productId, int quantity, int userId)
        {
            int orderId = 0;
            using (var ctx = new HoaHoeHoaSoiContext())
            {
                var order = GetCartByUserId(userId);
                if (order != null)
                {
                    orderId = order.Id;
                    var line = ctx.OrderLines.FirstOrDefault(ol => ol.ProductsId == productId && ol.OrderedId == order.Id);
                    if (line != null)
                    {
                        line.Quantity = quantity;
                        ctx.OrderLines.Update(line);
                        ctx.SaveChanges();
                    }
                }
            }

            UpdateOrderTotal(orderId);
        }

        public static void RemoveProductFromCart(int productId, int userId)
        {
            int orderId = 0;
            using (var ctx = new HoaHoeHoaSoiContext())
            {
                var order = GetCartByUserId(userId);
                if(order != null)
                {
                    orderId = order.Id;
                    var line = ctx.OrderLines.FirstOrDefault(ol => ol.ProductsId == productId && ol.OrderedId == order.Id);
                    if(line != null)
                    {
                        ctx.OrderLines.Remove(line);
                        ctx.SaveChanges();
                    }
                }
            }

            UpdateOrderTotal(orderId);
        }

        public static void ProcessCartIntoOrder(int userId, string name, string address, string phone, string momoId = "")
        {
            using(var ctx = new HoaHoeHoaSoiContext())
            {
                var order = GetCartByUserId(userId);
                if (order != null) 
                {
                    order.MomoOrderId = momoId;
                    order.PaymentStatus = (int)PaymentStatus.UnPaid;
                    order.ReceiverName = name;
                    order.ReceiverAddress = address;
                    order.ReceiverPhone = phone;
                    ctx.Ordereds.Update(order);
                    ctx.SaveChanges();
                }
            }
        }
    }
}
