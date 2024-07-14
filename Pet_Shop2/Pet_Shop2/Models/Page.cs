using System;
using System.Collections.Generic;

namespace Pet_Shop2.Models;

public partial class Page
{
    public int Id { get; set; }

    public string? PageName { get; set; }

    public string? Contents { get; set; }

    public string? Thumb { get; set; }

    public bool? Published { get; set; }

    public string? Title { get; set; }

    public string? MetaDesc { get; set; }

    public string? MetaKey { get; set; }

    public string? Alias { get; set; }

    public DateTime? CreateAt { get; set; }

    public virtual ICollection<Banner> Banners { get; set; } = new List<Banner>();

    public virtual ICollection<Slider> Sliders { get; set; } = new List<Slider>();
}
