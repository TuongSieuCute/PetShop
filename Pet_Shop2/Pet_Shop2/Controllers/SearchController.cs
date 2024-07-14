using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Pet_Shop2.Models;

namespace Pet_Shop2.Controllers
{
    public class SearchController : Controller
    {
        private readonly PetShopContext db;

        public SearchController(PetShopContext db)
        {
            this.db = db;
        }
        public IActionResult FindProductCus(string keyword = "",int condition=0)
        {
            List<Product> products = db.Products.ToList();
            if(!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                products = products.Where(x => x.ProductName.ToLower().Contains(keyword)).ToList();

            }
            if(condition !=0)
            {
                //Phổ biến nhất
                if (condition == 1)
                {
                    products = products.Where(x => x.Star != null & x.Star > 3).ToList();
                }
                //Mới nhất
                else if (condition == 2)
                {
                    products = products.Where(x =>x.IsHot==true).ToList();
                }
                //Gía cao đến thấp
                else if(condition == 3)
                {
                    products=products.OrderByDescending(x => x.Price).ToList();
                }
                //Gía thấp đến cao
                else if (condition == 4)
                {
                    products = products.OrderBy(x => x.Price).ToList();
                }
                products = products.Take(15).ToList();
            }    
            return PartialView("listproductsCussearchpartial", products);
        }
    }
}
