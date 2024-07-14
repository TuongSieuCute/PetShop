using System;
using System.Collections.Generic;

namespace Pet_Shop2.Models;

public partial class Location
{
    public int Id { get; set; }

    public string? Type { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<District> Districts { get; set; } = new List<District>();
}
