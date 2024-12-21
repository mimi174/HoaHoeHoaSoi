using System;
using System.Collections.Generic;

namespace HoaHoeHoaSoi.Data.Models;

public partial class Ordered
{
    public int Id { get; set; }

    public int? CustomerId { get; set; }

    public DateTime? Date { get; set; }

    public double? Total { get; set; }

    public int PaymentStatus { get; set; }

    public int? UserId { get; set; }

    public string? MomoOrderId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

    public virtual UserInfo? User { get; set; }
}
