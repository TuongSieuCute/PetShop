using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pet_Shop2.Models;

namespace Pet_Shop2.Controllers
{
    [Authorize]
    public class MyAccountController : Controller
    {
        PetShopContext db;
        public MyAccountController(PetShopContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            var CusID = HttpContext.Session.GetString("CustomerId");
            if (CusID != null)
            {
                ViewBag.Acc = db.Accounts.SingleOrDefault(x => x.Id == int.Parse(CusID));
                var Acc = db.Accounts.SingleOrDefault(x => x.Id == int.Parse(CusID));
                return View(Acc);
            }
            else
            {
                return RedirectToAction("login","cus_account");
            } 
                
            
        }
    }
}
