using System;
using System.Collections.Generic;

namespace HoaHoeHoaSoi.API.Models;

public partial class Product
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public double? Price { get; set; }

    public string? Description { get; set; }

    public string? Img { get; set; }

    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
}
