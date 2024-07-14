using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pet_Shop2.Helper;
using Pet_Shop2.Models;
using X.PagedList;
using AspNetCoreHero.ToastNotification.Abstractions;
namespace Pet_Shop2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [LoginRequired]
    public class ProductsController : Controller
    {
        PetShopContext db = new PetShopContext();
        public INotyfService notyfService { get; }
        private readonly IWebHostEnvironment _environment;
        public ProductsController(IWebHostEnvironment environment, INotyfService _notyfService)
        {
            _environment = environment;
            notyfService = _notyfService;
        }
        public IActionResult Index(int? page)
        {
            var pageNumber = page ?? 1;
            int pageSize = 15;
            var OnePage = db.Products.Include(x => x.Category).Where(x=>x.IsDelete==false).ToPagedList(pageNumber, pageSize);
            ViewBag.Category = db.Categories.ToList();
            return View(OnePage);
        }
        public IActionResult Create()
        {

            ViewBag.categories = db.Categories.ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product pro, IFormFile fThumb, List<IFormFile> childImage)
        {
            if (ModelState.IsValid)
            {
                if (pro.ProductName != null) pro.ProductName = Utilities.ToTitleCase(pro.ProductName);
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(fThumb.FileName);
                    string tmp = image.ToLower() + extension;
                    pro.Thumb = await Utilities.UploadFile(fThumb, @"products", tmp);
                }
                if (string.IsNullOrEmpty(pro.Thumb)) pro.Thumb = "default.jpg";
                pro.Alias = Utilities.SEOUrl(pro.ProductName == null ? "Default" : pro.ProductName);
                pro.CreatedDate = DateTime.Now;
                pro.ModifiedDate = DateTime.Now;
                db.Products.Add(pro);
                await db.SaveChangesAsync();

                for (int i = 0; i < childImage.Count(); i++)
                {
                    string extension = Path.GetExtension(childImage[i].FileName);
                    string image = Utilities.SEOUrl(childImage[i].FileName);
                    string tmp = image.ToLower() + extension;
                    ProductImage prdImage = new ProductImage()
                    {
                        Idproduct = pro.Id,
                        ImageName = await Utilities.UploadFile(childImage[i], @"Childproduct", tmp),
                    };
                    db.ProductImages.Add(prdImage);
                    db.SaveChanges();

                }


                notyfService.Success("Đã thêm mới thành công !");
                return RedirectToAction("Index");
            }
            ViewBag.categories = db.Categories.ToList();
            return View();
        }
        public IActionResult Edit(int id)
        {
            ViewBag.categories = db.Categories.ToList();
            ViewBag.ChildImage = db.ProductImages.Where(x => x.Idproduct == id);
            return View(db.Products.SingleOrDefault(x => x.Id == id));
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Product p, IFormFile fThumb, List<IFormFile> childImage)
        {
            var pro = db.Products.SingleOrDefault(x => x.Id == p.Id);
            if (pro != null)
            {
                if (pro.ProductName != null) pro.ProductName = Utilities.ToTitleCase(pro.ProductName);
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(fThumb.FileName);
                    string tmp = image.ToLower() + extension;
                    pro.Thumb = await Utilities.UploadFile(fThumb, @"products", tmp);
                }
                pro.Alias = Utilities.SEOUrl(pro.ProductName == null ? "Default" : pro.ProductName);
                pro.ShortDesc = p.ShortDesc;
                pro.Description = p.Description;
                pro.IsHot = p.IsHot;
                pro.Discount = p.Discount;
                pro.IsSale = p.IsSale;
                pro.Price = p.Price;
                pro.Quantity = p.Quantity;
                pro.Active = p.Active;
                pro.CategoryId = p.CategoryId;
                pro.ModifiedDate = DateTime.Now;
                await db.SaveChangesAsync();

                //Xóa đi các ảnh con cũ nếu có 
                if (childImage.Count() > 0)
                {
                    var lstChildImage = db.ProductImages.Where(x => x.Idproduct == p.Id).ToList();
                    db.ProductImages.RemoveRange(lstChildImage);
                    db.SaveChanges();
                    //Cập nhật ảnh mới vào db
                    for (int i = 0; i < childImage.Count(); i++)
                    {
                        string extension = Path.GetExtension(childImage[i].FileName);
                        string image = Utilities.SEOUrl(childImage[i].FileName);
                        string tmp = image.ToLower() + extension;
                        ProductImage prdImage = new ProductImage()
                        {
                            Idproduct = pro.Id,
                            ImageName = await Utilities.UploadFile(childImage[i], @"Childproduct", tmp),
                        };
                        db.ProductImages.Add(prdImage);
                        db.SaveChanges();

                    }
                }
                notyfService.Success("Chỉnh sửa thành công !");
                return RedirectToAction("Index");
            }
            ViewBag.categories = db.Categories.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var tmp = db.Products.SingleOrDefault(x => x.Id == id);
            if (tmp != null)
            {
                db.Products.Remove(tmp);
                db.SaveChanges();
                notyfService.Success("Đã xóa thành công !");
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        public IActionResult IndexComment(int? page)
        {

            var pageNumber = page ?? 1;
            int pageSize = 15;
            var OnePage = db.ProductRates.ToPagedList(pageNumber, pageSize);
            return View(OnePage);
        }
        public IActionResult DeleteComment(int id)
        {
            var tmp = db.ProductRates.SingleOrDefault(x => x.Id == id);
            if (tmp != null)
            {
                db.ProductRates.Remove(tmp);
                db.SaveChanges();
                notyfService.Success("Sửa thành công");
                return RedirectToAction("IndexComment");
            }
            else
            {
                notyfService.Error("Sửa không thành công");
                return RedirectToAction("IndexComment");
            }

        }
        public IActionResult Track(int? page)
        {

            var pageNumber = page ?? 1;
            int pageSize = 15;
            var OnePage = db.Products.Include(x => x.Category).Where(x=>x.IsDelete==true).ToPagedList(pageNumber, pageSize);
            ViewBag.Category = db.Categories.ToList();
            return View(OnePage);
        }
        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            Product? tmp = db.Products.SingleOrDefault(p => p.Id == id);
            if (tmp != null)
            {
                tmp.IsDelete = true;
                tmp.Active = false;
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new {success=false});
        }
        [HttpPost]
        public IActionResult RestoreProduct(int id)
        {
            Product? tmp = db.Products.SingleOrDefault(p => p.Id == id);
            if (tmp != null)
            {
                tmp.IsDelete = false;
                tmp.Active = true;
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

    }
}
