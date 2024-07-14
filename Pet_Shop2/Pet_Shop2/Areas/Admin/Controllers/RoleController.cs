using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pet_Shop2.Models;
using X.PagedList;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace Pet_Shop2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [LoginRequired]
    public class RoleController : Controller
    {
        PetShopContext db ;
        INotyfService notyfService;

        public RoleController(PetShopContext db, INotyfService notyfService)
        {
            this.db = db;
            this.notyfService = notyfService;
        }
    
        public IActionResult Index(int? page)
        {
            var pageNumber = page ?? 1;
            int pageSize = 5;
            // Lấy danh sách các vai trò từ database và chuyển đổi thành danh sách IdentityRole
            var rolesFromDb = db.Roles.ToPagedList(pageNumber,pageSize);
            return View(rolesFromDb);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Role role)
        {
            if (role != null)
            {
                db.Roles.Add(role);
                db.SaveChanges();
                notyfService.Success("Tạo mới vai trò thành công !");
                return RedirectToAction("Index");
            }
            else
            return View(role);
        }
        public IActionResult Edit(int? id)
        {

            var role = db.Roles.SingleOrDefault(x => x.Id == id);
            if (role != null)
            {
                return View(role);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Edit(Role role)
        {
            var tmp = db.Roles.SingleOrDefault(x => x.Id == role.Id);
            if (tmp != null)
            {
                tmp.RoleName = role.RoleName;
                tmp.Description = role.Description;
                notyfService.Success("Tạo mới vai trò thành công !");
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(role);
        }
        [HttpPost]
        public ActionResult Delete(int? id)
        {
            var tmp = db.Roles.SingleOrDefault(x => x.Id == id);
            var lstAccount = db.Accounts.Where(x=>x.RoleId==id).ToList();
            try
            {
                if (tmp != null && lstAccount.Count()==0)
                {
                   /* db.Roles.Remove(tmp);
                    db.SaveChanges();*/
                    notyfService.Success("Xóa thành công");
                    return View("Index");
                }
                else
                {
                    notyfService.Error("Đã có người sử dụng Role này, không thể xóa !");
                    return View("index");
                }
                
            }
            catch (Exception ex)
            {
                notyfService.Error("Có tài khoản đã dùng quyền này. Không thể xóa");
                return View("Index");
            }
           
        }
    }
}
