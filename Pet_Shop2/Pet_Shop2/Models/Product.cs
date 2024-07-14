using System;
using System.Collections.Generic;

namespace Pet_Shop2.Models;

public partial class Product
{
    public int Id { get; set; }

    public string? ProductName { get; set; }

    public string? ShortDesc { get; set; }

    public string? Description { get; set; }

    public int? CategoryId { get; set; }

    public string? Alias { get; set; }

    public int? Quantity { get; set; }

    public int? Price { get; set; }

    public int? Discount { get; set; }

    public int? Liked { get; set; }

    public int? Star { get; set; }

    public string? Thumb { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool? IsHot { get; set; }

    public bool? IsSale { get; set; }

    public bool? Active { get; set; }

    public bool? IsDelete { get; set; }

    public string? MetaKey { get; set; }

    public string? MetaDesc { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
}
