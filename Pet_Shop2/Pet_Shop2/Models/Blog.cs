using System;
using System.Collections.Generic;

namespace Pet_Shop2.Models;

public partial class Blog
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Scontents { get; set; }

    public string? Contents { get; set; }

    public string? Thumb { get; set; }

    public bool? Published { get; set; }

    public string? Alias { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? AuthorId { get; set; }

    public string? Tags { get; set; }

    public int? Views { get; set; }

    public string? MetaDesc { get; set; }

    public string? MetaKey { get; set; }
}
