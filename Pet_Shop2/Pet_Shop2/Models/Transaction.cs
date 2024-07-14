using System;
using System.Collections.Generic;

namespace Pet_Shop2.Models;

public partial class Transaction
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
