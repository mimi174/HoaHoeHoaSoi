using System;
using System.Collections.Generic;

namespace HoaHoeHoaSoi.API.Models;

public partial class OrderLine
{
    public int Id { get; set; }

    public int OrderedId { get; set; }

    public int ProductsId { get; set; }

    public double? Price { get; set; }

    public int? Quantity { get; set; }

    public virtual Ordered Ordered { get; set; } = null!;

    public virtual Product Products { get; set; } = null!;
}
