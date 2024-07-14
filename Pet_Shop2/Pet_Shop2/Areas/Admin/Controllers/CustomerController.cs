using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Pet_Shop2.Models;
using X.PagedList;

namespace Pet_Shop2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [LoginRequired]
    public class CustomerController : Controller
    {
        private readonly PetShopContext db;
        public INotyfService NotyfService { get; }

        public CustomerController(PetShopContext db, INotyfService notyfService)
        {
            this.db = db;
            this.NotyfService = notyfService;
        }
        public IActionResult Index(int? page)
        {
            var pageNumber = page ?? 1;
            int pageSize = 10;
            var OnePage = db.Accounts.ToPagedList(pageNumber, pageSize);
            return View(OnePage);
        }
    }
}
