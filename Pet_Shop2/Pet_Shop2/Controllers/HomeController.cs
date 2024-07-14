using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pet_Shop2.Models;
using System.Diagnostics;

namespace Pet_Shop2.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        PetShopContext db;
        INotyfService notyfService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, PetShopContext db,INotyfService notyfService)
        {
            _logger = logger;
            this.notyfService = notyfService;
            this.db = db;   
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var CusID = HttpContext.Session.GetString("CustomerId");
            if (CusID != null)
                ViewBag.Acc = db.Accounts.SingleOrDefault(x=>x.Id==int.Parse(CusID));
            int IDpage = 1;
            ViewBag.SliderHome = db.Sliders.Include(x=>x.Page).Where(x=>x.PageId==IDpage).ToList();
            ViewBag.Category = db.Categories.ToList();
            ViewBag.Banner = db.Banners.Take(4).ToList();
            ViewBag.ProductSuggest = db.Products.OrderByDescending(x => x.Star).Take(3).ToList();
            ViewBag.ProductRecently = db.Products.OrderByDescending(x=>x.Star).Take(10).ToList();
            return View(db.Products.Where(x => x.Active == true).Take(15).ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("/StatusError/{statusCode}")]
        [AllowAnonymous]
        public IActionResult Error(int statusCode)
        {
            if (statusCode == 404)
            {
                //xữ lý lỗi trang
                return View();
            }
            return View();
        }
    }
}