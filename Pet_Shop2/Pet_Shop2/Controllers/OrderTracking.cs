using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pet_Shop2.Models;

namespace Pet_Shop2.Controllers
{
    [Authorize]
    public class OrderTracking : Controller
    {
        PetShopContext db;

        public OrderTracking(PetShopContext db)

        {
            this.db = db;
        }

        public IActionResult Index()
        {
            var CusID = HttpContext.Session.GetString("CustomerId");
            if (CusID != null)
                ViewBag.Acc = db.Accounts.SingleOrDefault(x => x.Id == int.Parse(CusID));

            if(CusID != null)
            {
                var accountID = db.Accounts.SingleOrDefault(x => x.Id == int.Parse(CusID))?.Id;


                var ordertracking = db.Orders
                  .Include(x => x.OrderDetails)
                  .Where(x => x.AccountId == accountID)
                  .ToList();
                return View(ordertracking);
            }    
            
            return View();
        }
        public IActionResult Details(int OrDerID)

        {
            var CusID = HttpContext.Session.GetString("CustomerId");
            if (CusID != null)
                ViewBag.Acc = db.Accounts.SingleOrDefault(x => x.Id == int.Parse(CusID));
            var lstOrderDetails = db.OrderDetails.Where(x => x.OrderId == OrDerID).ToList();

            List<Product> lstProduct = new List<Product>();
            foreach (var item in lstOrderDetails)
            {
                var tmp = db.Products.SingleOrDefault(x => x.Id == item.ProductId);
                if (tmp != null)
                {
                    lstProduct.Add(tmp);
                }
            }
            ViewBag.lstProduct = lstProduct;
            return View(lstOrderDetails);

        }
    }
}
