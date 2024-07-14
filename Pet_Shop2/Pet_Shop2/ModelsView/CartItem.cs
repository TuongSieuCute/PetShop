
using Pet_Shop2.Models;

namespace Pet_Shop2.ModelsView
{
    public class CartItem
    {
        public Product? product { get; set; }
        public int amount { get; set; }
        public double TotalMoney => amount * (product.Price.HasValue ? (double)product.Price : 0.0);
    }
}
