using Microsoft.AspNetCore.Mvc;
using Pet_Shop2.Helper;

namespace Pet_Shop2.Controllers
{
    public class NewsController : Controller
    {
        public IActionResult Index()
        {
            string url = "https://vnexpress.net/rss/tin-moi-nhat.rss";
            ViewBag.rssItems = RssHelper.Read(url);
            return View();
        }
    }
}
