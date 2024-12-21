using System;
using System.Collections.Generic;

namespace HoaHoeHoaSoi.Data.Models;

public partial class Feedback
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Content { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }
}
