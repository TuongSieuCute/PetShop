using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pet_Shop2.Helper;
using Pet_Shop2.Models;
//Phân trang
using X.PagedList;
//Thông báo
using AspNetCoreHero.ToastNotification.Abstractions;
//Tách chữ trong html
using HtmlAgilityPack;
namespace Pet_Shop2.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogController : Controller
    {
        PetShopContext db = new PetShopContext();
        public INotyfService notyfService { get; }
        private readonly IWebHostEnvironment _environment;
        public BlogController(IWebHostEnvironment environment, INotyfService _notyfService)
        {
            _environment = environment;
            notyfService = _notyfService;
        }
        public IActionResult Index(int? page)
        {
            var pageNumber = page ?? 1;
            int pageSize = 10;
            var OnePage = db.Blogs.ToPagedList(pageNumber, pageSize);
            return View(OnePage);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Blog blog, IFormFile fThumb)
        {
            if (ModelState.IsValid)
            {
                if (blog.Title != null) blog.Title = Utilities.ToTitleCase(blog.Title);
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(fThumb.FileName);
                    string tmp = image.ToLower() + extension;
                    blog.Thumb = await Utilities.UploadFile(fThumb, @"blogs", tmp);
                }
                if (string.IsNullOrEmpty(blog.Thumb)) blog.Thumb = "default.jpg";
                //string[] sct = blog.Contents != null ? blog.Contents.Split(' ').Take(5).ToArray() : "Bạn chưa viết nội dung".Split(' ').Take(5).ToArray();
                if(!string.IsNullOrEmpty(blog.Contents))
                blog.Scontents = Utilities.GetTextFromHtml(blog.Contents) + " ...";
                blog.Alias = Utilities.SEOUrl(blog.Title == null ? "Default" : blog.Title);
                blog.CreatedDate = DateTime.Now;
                blog.Views = 0;
                db.Blogs.Add(blog);
                await db.SaveChangesAsync();
                notyfService.Success("Đã thêm mới thành công !");
                return RedirectToAction("Index");
            }
            notyfService.Error("Thêm mới không thành công !");
            return View(blog);
        }
        public IActionResult Edit(int id)
        {
            var tmp = db.Blogs.SingleOrDefault(x => x.Id == id);
            return View(tmp);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Blog bl, IFormFile fThumb)
        {
            var blog = db.Blogs.SingleOrDefault(db => db.Id == bl.Id);
            if (blog!=null)
            {
                if (bl.Title != null) blog.Title = Utilities.ToTitleCase(bl.Title);
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(fThumb.FileName);
                    string tmp = image.ToLower() + extension;
                    blog.Thumb = await Utilities.UploadFile(fThumb, @"blogs", tmp);
                }
                if (string.IsNullOrEmpty(blog.Thumb)) blog.Thumb = "default.jpg";
                blog.Contents = bl.Contents;
                //string[] sct = bl.Contents != null ? bl.Contents.Split(' ').Take(5).ToArray() : "Bạn chưa viết nội dung".Split(' ').Take(5).ToArray();
                string strtmp = string.Join(' ', Utilities.GetTextFromHtml(bl.Contents ==null? "Chưa có mô tả nào":bl.Contents));
                List<string> list = strtmp.Split(' ').ToList();
                string strscontent=string.Join(' ', list.Take(10));
                if (!string.IsNullOrEmpty(strscontent))
                    blog.Scontents = strscontent + " ...";
               
                
                blog.Alias = Utilities.SEOUrl(bl.Title == null ? "Default" : bl.Title);
                blog.Views = bl.Views;
                blog.Published = bl.Published;
                await db.SaveChangesAsync();
                notyfService.Success("Chỉnh sửa thành công !");
                return RedirectToAction("Index");
            }
            notyfService.Error("Chỉnh sửa không thành công !");
            return View(blog);
        }
        public IActionResult Delete(int id)
        {
            var tmp = db.Blogs.SingleOrDefault(x => x.Id == id);

            if (tmp != null)
            {
                db.Blogs.Remove(tmp);
                db.SaveChanges();
                notyfService.Error("Xóa thành công !");
                return RedirectToAction("Index");
            }
            notyfService.Error("Xóa không thành công");
            return RedirectToAction("Index");
        }
    }
}
