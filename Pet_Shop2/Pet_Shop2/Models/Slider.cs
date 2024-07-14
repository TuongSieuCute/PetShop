using System;
using System.Collections.Generic;

namespace Pet_Shop2.Models;

public partial class Slider
{
    public int Id { get; set; }

    public int? Index { get; set; }

    public string? Image { get; set; }

    public int? PageId { get; set; }

    public string? Link { get; set; }

    public string? Content { get; set; }

    public bool? Top { get; set; }

    public bool? Bottom { get; set; }

    public bool? Center { get; set; }

    public bool? Left { get; set; }

    public bool? Right { get; set; }

    public virtual Page? Page { get; set; }
}
