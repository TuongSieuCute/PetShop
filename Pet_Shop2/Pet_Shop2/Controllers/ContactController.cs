using Microsoft.AspNetCore.Mvc;
using Pet_Shop2.Models;

namespace Pet_Shop2.Controllers
{
    public class ContactController : Controller
    {
        PetShopContext db;
        public ContactController(PetShopContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            var CusID = HttpContext.Session.GetString("CustomerId");
            if (CusID != null)
                ViewBag.Acc = db.Accounts.SingleOrDefault(x => x.Id == int.Parse(CusID));
            return View();
        }
    }
}
