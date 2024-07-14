using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Pet_Shop2.Controllers
{
    public class AjaxContentController : Controller
    {
        INotyfService _otyfService;
        public AjaxContentController(INotyfService otyfService)
        {
            _otyfService = otyfService;
        }
        public IActionResult HeaderCart()
        {
            return ViewComponent("NumberCart");
        }       
    }
}
