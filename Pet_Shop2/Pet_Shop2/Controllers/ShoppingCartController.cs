 using AspNetCoreHero.ToastNotification.Abstractions;

using Microsoft.AspNetCore.Http.Extensions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pet_Shop2.Models;
using Pet_Shop2.ModelsView;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;

namespace Pet_Shop2.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly PetShopContext db;
        public INotyfService notyfService { get; }


        public ShoppingCartController(PetShopContext db, INotyfService notyfService)
        {
            this.db = db;
            this.notyfService = notyfService;

        }
        public List<CartItem> GioHang
        {
            get
            {
                var gh = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if (gh == default(List<CartItem>))
                {
                    gh = new List<CartItem>();
                }
                return gh;
            }
        }
        [HttpPost]
        [AllowAnonymous]
        public IActionResult AddToCart(int productID, int? amount)
        {
            List<CartItem> gioHang = GioHang;
            //Thêm sản phẩm vào giỏ hàng
            CartItem? item = gioHang.SingleOrDefault(x => x.product?.Id == productID);
            if (item != null) // da ton tai => cap nhat so luong
            {
                if (amount.HasValue) item.amount += amount.Value;
                else item.amount++;
            }
            else
            {
                Product? prd = db.Products.SingleOrDefault(x=>x.Id== productID);
                if(prd != null)
                {
                    item = new CartItem
                    {
                        product=prd,
                        amount=amount.HasValue? amount.Value:1,
                    };
                    gioHang.Add(item);
                }
            }
        
            
            //Lưu lại session
            HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
            //notyfService.success("Thêm sản phẩm thành công !");
            /*return PartialView("Default", gioHang.Count());*/
            return Json(new { success = true });

        }
        [HttpPost]
        [AllowAnonymous]
        public IActionResult ReduceToCart(int productID, int? amount)
        {
            List<CartItem> gioHang = GioHang;
            //Thêm sản phẩm vào giỏ hàng
            CartItem? item = gioHang.SingleOrDefault(x => x.product?.Id == productID);
            if (item != null) // da ton tai => cap nhat so luong
            {
                if (item.amount>1) item.amount -= amount.Value;
        
            }
            //Lưu lại session
            HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
            //notyfService.success("Thêm sản phẩm thành công !");
            return Json(new { success = true });

        }
        [HttpPost]
        [AllowAnonymous]
        public IActionResult RemoveProduct(int productID)
        {
            List<CartItem> gioHang=GioHang;
            if(gioHang != null)
            {
                CartItem? item = gioHang.SingleOrDefault(x=>x.product?.Id == productID);
                if(item != null)
                {
                    gioHang.Remove(item);
                }
            }
            //Lưu lại session
            HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
            return Json(new {success = true});
        }
        [HttpPost]
        [AllowAnonymous]
        public IActionResult RemoveCart()
        {
            List<CartItem> gioHang = GioHang;
            if (gioHang != null)
            {
                gioHang = null;
            }
            //Lưu lại session
            HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
            return Json(new { success = true });
        }
        [AllowAnonymous]
        public IActionResult Index()
        {

            var CusID = HttpContext.Session.GetString("CustomerId");
            if (CusID != null)
                ViewBag.Acc = db.Accounts.SingleOrDefault(x => x.Id == int.Parse(CusID));
            var lsCart = GioHang;
            ViewBag.CusID = HttpContext.Session.GetString("CustomerId");
            return View(lsCart);
        }
        [AllowAnonymous]
        public IActionResult QuantityCart()
        {
            var lsCart = GioHang;
            return PartialView("~/Views/Shared/Components/NumberCart/Default.cshtml",lsCart.Count());
        }
    }
}
