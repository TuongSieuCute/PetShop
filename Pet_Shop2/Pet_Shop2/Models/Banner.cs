using System;
using System.Collections.Generic;

namespace Pet_Shop2.Models;

public partial class Banner
{
    public int Id { get; set; }

    public int? PageId { get; set; }

    public string? Image { get; set; }

    public string? Title { get; set; }

    public virtual Page? Page { get; set; }
}
