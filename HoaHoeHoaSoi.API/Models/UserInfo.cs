using System;
using System.Collections.Generic;

namespace HoaHoeHoaSoi.API.Models;

public partial class UserInfo
{
    public int Id { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Name { get; set; }

    public DateTime? Dob { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Avatar { get; set; }

    public bool? Gender { get; set; }

    public string? Mail { get; set; }
}
