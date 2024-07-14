using System;
using System.Collections.Generic;

namespace Pet_Shop2.Models;

public partial class ProductRate
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public int? IdCus { get; set; }

    public string? CusName { get; set; }
}
