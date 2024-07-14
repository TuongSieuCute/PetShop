using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pet_Shop2.Helper;
using Pet_Shop2.Models;
using X.PagedList;
using AspNetCoreHero.ToastNotification.Abstractions;
using Pet_Shop2.ModelsView;
using Pet_Shop2.Areas.Admin.Models;
using NuGet.Packaging.Signing;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Pet_Shop2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [LoginRequired]
    public class PageController : Controller
    {
        private readonly PetShopContext db;
        public INotyfService NotyfService { get; }
        public IWebHostEnvironment _environment;
        public PageController(PetShopContext db, IWebHostEnvironment webHostEnvironment, INotyfService NotyfService)
        {
            this.db = db;
            _environment = webHostEnvironment;
            this.NotyfService = NotyfService;
        }
        public IActionResult Index(int? page)
        {
            var pageNumber = page ?? 1;
            int pageSize = 10;
            var OnePage = db.Pages.ToPagedList(pageNumber, pageSize);
            return View(OnePage);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Page page, IFormFile imageFile)
        {
           
                page.CreateAt = DateTime.Now;
                if (page.Alias != null) page.Alias = Utilities.SEOUrl(page.Alias);
                else page.Alias = Utilities.SEOUrl(page.Title == null ? "default" : page.Title);
                if (imageFile != null)
                {
                    string extension = Path.GetExtension(imageFile.FileName);
                    string image = Utilities.SEOUrl(imageFile.FileName);
                    string tmp = image.ToLower() + extension;
                    page.Thumb = await Utilities.UploadFile(imageFile, @"pages", tmp);
                }
                db.Pages.Add(page);
                db.SaveChanges();
                NotyfService.Success("Thêm thành công trang mới");
                return RedirectToAction("Index");
           
        }
        public IActionResult Edit(int id)
        {
            var tmp = db.Pages.SingleOrDefault(x => x.Id == id);
            return View(tmp);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Page p, IFormFile imageFile)
        {
            var page = await db.Pages.SingleOrDefaultAsync(x => x.Id == p.Id);


            if (page != null)
            {
                if (page.Alias != null) page.Alias = Utilities.SEOUrl(page.Alias);
                else page.Alias = Utilities.SEOUrl(page.Title == null ? "default" : page.Title);
                if (imageFile != null)
                {
                    string extension = Path.GetExtension(imageFile.FileName);
                    string image = Utilities.SEOUrl(imageFile.FileName);
                    string tmp = image.ToLower() + extension;
                    page.Thumb = await Utilities.UploadFile(imageFile, @"pages", tmp);
                }
                page.Title = p.Title;
                page.Published = p.Published;
                page.Contents = p.Contents;
                db.SaveChanges();
                NotyfService.Success("Chỉnh sửa thành công trang");
                return RedirectToAction("Index");
            }



            NotyfService.Error("Chỉnh sửa trang thất bại !");
            return View(page);
        }
        [HttpPost]
        public IActionResult Delete(int? id)
        {
            if (ModelState.IsValid)
            {
                var tmp = db.Pages.SingleOrDefault(x => x.Id == id);
                if (tmp != null)
                {
                    db.Remove(tmp);
                    db.SaveChanges();
                    NotyfService.Success("Xóa trang thành công");
                    return RedirectToAction("Index");
                }

            }
            NotyfService.Error("Xóa trang thất bại");
            return RedirectToAction("Index");
        }
        public List<CartSlider> TrinhChieu
        {
            get
            {
                var slider = HttpContext.Session.Get<List<CartSlider>>("TrinhChieu");
                if (slider == default(List<CartSlider>))
                {
                    slider = new List<CartSlider>();
                }
                return slider;
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddToImageSlider(List<IFormFile> addressimage)
        {
            List<CartSlider> lstSlider = TrinhChieu;
            if (addressimage != null)
            {
                int index = 1;
                foreach (var item in addressimage)
                {
                    string extension = Path.GetExtension(item.FileName);
                    string image = Utilities.SEOUrl(item.FileName);
                    string tmp = image.ToLower() + extension;
                    CartSlider slider = new CartSlider()
                    {
                        index = index,
                        image = await Utilities.UploadFile(item, @"Sliders", tmp),
                        link = null,
                        bottom = false,
                        left = false,
                        right = false,
                        top = false,
                        center = false
                    };
                    index++;
                    lstSlider.Add(slider);
                }
            }
            //Lưu lại session
            HttpContext.Session.Set<List<CartSlider>>("TrinhChieu", lstSlider);
            return Json(new { success = true });

        }
        [HttpPost]
        public IActionResult AddToDetailSlider( List<CartSlider>? sliders)
        {
            if(ModelState.IsValid)
            {
                List<CartSlider> lstSlider = TrinhChieu;
                {
                    if (sliders != null)
                    {
                        for (int i = 0; i < sliders.Count; i++)
                        {
                            lstSlider[i].content = sliders[i].content;
                            lstSlider[i].link = sliders[i].link;
                            lstSlider[i].bottom = sliders[i].bottom;
                            lstSlider[i].left = sliders[i].left;
                            lstSlider[i].right = sliders[i].right;
                            lstSlider[i].top = sliders[i].top;
                            lstSlider[i].center = sliders[i].center;
                        }
                    }
                }
                //Lưu lại session
                HttpContext.Session.Set<List<CartSlider>>("TrinhChieu", lstSlider);
                return Json(new { success = true });
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                return Json(new { success = false });
            } 
        }
        public IActionResult changeSlider()
        {
            //Xóa hết các slider cũ trong db
            var sliders = db.Sliders.ToList();
            foreach(var item in  sliders)
            {
                db.Sliders.Remove(item);
                db.SaveChanges();
            }    
            //Lấy slider mới
            var slider = TrinhChieu;
            foreach(var item in slider)
            {
                Slider slider1 = new Slider()
                {
                    Index = item.index,
                    PageId = 1,
                    Image = item.image,
                    Content = item.content,
                    Left = item.left,
                    Right = item.right,
                    Top = item.top,
                    Bottom = item.bottom,
                    Center = item.center,
                    
                };
                db.Sliders.Add(slider1);
                db.SaveChanges();
            }//Quy trình thác nước, qt tăng trưởng , quy trình xoắn ốc
            slider = null;
            //Lưu lại 
            HttpContext.Session.Set<List<CartSlider>>("TrinhChieu", slider);
            return Json(new { success = true });
        }
        [HttpPost]
        public async Task<IActionResult> SaveBanner(List<IFormFile> files)
        {
            if(files!=null)
            {
                foreach (var item in files)
                {
                    string extension = Path.GetExtension(item.FileName);
                    string image = Utilities.SEOUrl(item.FileName);
                    string tmp = image.ToLower() + extension;
                    Banner bn = new Banner()
                    {
                        Title = "",
                        Image = await Utilities.UploadFile(item, @"banners", tmp),
                        PageId = 1,
                        
                    };
                    db.Banners.Add(bn);
                    db.SaveChanges();
                }
            }
            NotyfService.Success("Sửa banner thành công !");
            return RedirectToAction("trangchu", "page");
        }
        
        public IActionResult Trangchu()
        {
            var slider = TrinhChieu;
            return View(slider);
        }
    }
}
