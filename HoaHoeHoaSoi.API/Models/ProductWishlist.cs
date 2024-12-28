using System;
using System.Collections.Generic;

namespace HoaHoeHoaSoi.API.Models;

public partial class ProductWishlist
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual UserInfo User { get; set; } = null!;
}
