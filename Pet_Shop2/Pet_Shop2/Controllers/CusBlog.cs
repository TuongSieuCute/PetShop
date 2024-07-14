using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pet_Shop2.Models;
using X.PagedList;
using AspNetCoreHero.ToastNotification.Abstractions;
namespace Pet_Shop2.Controllers
{
    public class CusBlog : Controller
    {
        private readonly PetShopContext db;

        public CusBlog(PetShopContext context)
        {
            this.db = context;
        }
        public IActionResult Index(int? page)
        {
            var CusID = HttpContext.Session.GetString("CustomerId");
            if (CusID != null)
                ViewBag.Acc = db.Accounts.SingleOrDefault(x => x.Id == int.Parse(CusID));
            var pageNumber = page ?? 1;
            int pageSize = 9;
            var OnePage = db.Blogs.ToPagedList(pageNumber, pageSize);
            return View(OnePage);
        }
        public IActionResult Detail(int id)
        {
            var CusID = HttpContext.Session.GetString("CustomerId");
            if (CusID != null)
                ViewBag.Acc = db.Accounts.SingleOrDefault(x => x.Id == int.Parse(CusID));
            ViewBag.ListBlog = db.Blogs.Take(4).ToList();
            var tmp = db.Blogs.SingleOrDefault(x=>x.Id == id);
            if(tmp!= null)
            {
                tmp.Views += 1;
                db.SaveChanges();
            }    
            return View(tmp);
        }
    }
}
