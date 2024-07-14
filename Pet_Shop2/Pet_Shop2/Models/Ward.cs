using System;
using System.Collections.Generic;

namespace Pet_Shop2.Models;

public partial class Ward
{
    public int? WardId { get; set; }

    public int? DistrictId { get; set; }

    public string? Name { get; set; }

    public virtual District? District { get; set; }
}
