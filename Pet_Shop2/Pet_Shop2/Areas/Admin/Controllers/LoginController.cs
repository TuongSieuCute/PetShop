using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Pet_Shop2.Extensions;
using Pet_Shop2.Models;
using System.Security.Claims;

namespace Pet_Shop2.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController : Controller
    {
        PetShopContext db;
        INotyfService notyfService;
        public LoginController(PetShopContext db, INotyfService notyfService)
        {
            this.db = db;
            this.notyfService = notyfService;   
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(Account acc)
        {

            var ac = db.Accounts.SingleOrDefault(x=>x.UserName==acc.UserName||x.Email==acc.UserName);
            if(ac!=null)
            {
                //Kiểm tra password
                if(ac.Password==acc?.Password?.ToMD5())
                {
                    //Quyền admin
                    if (ac.RoleId == 1)
                    {
                        var claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.Name, ac.UserName));
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                        var identity = new ClaimsIdentity(claims, "Cookies");
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(principal);
                        notyfService.Success("Đăng nhập thành công");
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                }
                notyfService.Error("Thông tin tài khoản chưa đúng");
                return View();

                
            }
            else
            {
                notyfService.Error("Thông tin tài khoản chưa đúng");
                return View();
            }    
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Kiểm tra nếu người dùng hiện tại có quyền admin
            if (User.IsInRole("Admin"))
            {
                await HttpContext.SignOutAsync();
                notyfService.Success("Đã đăng xuất thành công");
                return RedirectToAction("Index", "login", new {area="Admin"});
            }

            
            return RedirectToAction("index", "login");
        }
    }
}
