using Microsoft.AspNetCore.Mvc;
using Pet_Shop2.ModelsView;

namespace Pet_Shop2.Controllers.Component
{
    public class NumberCartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            int quantity = 0;
            if(cart != null)
            {
                quantity = cart.Count();
            }
            return View(quantity);
        }
        
    }
}
