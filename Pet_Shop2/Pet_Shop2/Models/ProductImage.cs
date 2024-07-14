using System;
using System.Collections.Generic;

namespace Pet_Shop2.Models;

public partial class ProductImage
{
    public int Id { get; set; }

    public string? ImageName { get; set; }

    public int? Idproduct { get; set; }

    public virtual Product? IdproductNavigation { get; set; }
}
