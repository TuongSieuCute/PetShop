using System;
using System.Collections.Generic;

namespace Pet_Shop2.Models;

public partial class District
{
    public int Id { get; set; }

    public int? LocationId { get; set; }

    public string? Name { get; set; }

    public virtual Location? Location { get; set; }
}
