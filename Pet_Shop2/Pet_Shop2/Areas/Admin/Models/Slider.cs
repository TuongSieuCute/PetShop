namespace Pet_Shop2.Areas.Admin.Models
{
    public class CartSlider
    {
        public int index { get; set; }
        public string? image { get; set; }
        public string? link { get; set; }
        public string? content { get; set; }
        public bool top { get; set; }
        public bool center { get; set; }
        public bool bottom { get; set; }
        public bool right { get; set; }
        public bool left { get; set; }

    }
}
