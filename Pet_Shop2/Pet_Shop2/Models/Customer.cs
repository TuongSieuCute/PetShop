using System;
using System.Collections.Generic;

namespace Pet_Shop2.Models;

public partial class Customer
{
    public int Id { get; set; }

    public string? Fullname { get; set; }

    public DateTime? Birthday { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Location { get; set; }

    public string? District { get; set; }

    public string? Ward { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
