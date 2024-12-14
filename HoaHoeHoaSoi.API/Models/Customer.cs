using System;
using System.Collections.Generic;

namespace HoaHoeHoaSoi.API.Models;

public partial class Customer
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Ordered> Ordereds { get; set; } = new List<Ordered>();
}
