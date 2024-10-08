﻿using Microsoft.AspNetCore.Mvc;
using Pet_Shop2.Models;
using X.PagedList;
//Thông báo
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Pet_Shop2.Controllers
{
    public class CusProduct : Controller
    {
        private readonly PetShopContext db;
        public INotyfService notyfService { get; }

        public CusProduct(PetShopContext db, INotyfService notyfService)
        {
            this.db = db;
            this.notyfService = notyfService;   
        }
        
        public IActionResult Index(int ?page,int idCate=-1)
        {
            var CusID = HttpContext.Session.GetString("CustomerId");
            if (CusID != null)
                ViewBag.Acc = db.Accounts.SingleOrDefault(x => x.Id == int.Parse(CusID));
            try
            {
                var pageNumber = page ?? 1;
                int pageSize = 15;
               
                if (idCate!=-1)
                {
                    var OnePage= db.Products.Where(x => x.CategoryId == idCate).Include(x=>x.Category).Where(x=>x.Active==true).ToPagedList(pageNumber, pageSize);
                    return View(OnePage);
                }
                else
                {
                    var OnePage = db.Products.Include(x => x.Category).Where(x => x.Active == true).ToPagedList(pageNumber, pageSize);
                    return View(OnePage);
                }


            }
            catch
            {
                notyfService.Error("Lỗi trang. Vui lòng kiểm tra lại !");
                return RedirectToAction("Index");
            }
            
        }
       
        public IActionResult List(int CatID, int ? page)
        {

            try
            {
                var pageNumber = page ?? 1;
                int pageSize = 10;
                var OnePage = db.Products
                    .Where(x=>x.CategoryId == CatID)
                    .OrderByDescending(x=>x.Id)
                    .ToPagedList(pageNumber, pageSize);
                return View(OnePage);
            }
            catch 
            {
                notyfService.Error("Lỗi trang. Vui lòng kiểm tra lại !");
                return RedirectToAction("Index");
            }
        }

        [HttpGet("chi-tiet-san-pham/{product-name}-{id}")]
        public IActionResult Detail(int? id)
        {
            if (id == null)
            {
                notyfService.Error("Lỗi trang. Vui lòng kiểm tra lại !");
                return RedirectToAction("Index");
            }

            var CusID = HttpContext.Session.GetString("CustomerId");
            if (CusID != null)
            {
                ViewBag.Acc = db.Accounts.SingleOrDefault(x => x.Id == int.Parse(CusID));
            }

            var product = db.Products
                            .Include(x => x.Category)
                            .SingleOrDefault(x => x.Id == id);

            if (product == null)
            {
                notyfService.Error("Sản phẩm không tồn tại.");
                return RedirectToAction("Index");
            }

            // Load related images and comments
            ViewBag.ChildImage = db.ProductImages.Where(x => x.Idproduct == id).ToList();
            ViewBag.Comment = db.ProductRates.Where(x => x.ProductId == id).ToList();

            // Set up Open Graph meta tags and view data
            ViewData["Title"] = product.ProductName;
            ViewData["Description"] = product.Description;
            ViewData["OgTitle"] = product.ProductName;
            ViewData["OgDescription"] = product.Description;
            ViewData["OgImage"] = $"{Request.Scheme}://{Request.Host}{Url.Content($"~/FolderImages/products/{product.Thumb}")}";
            ViewData["OgUrl"] = Url.Action("Detail", "CusProduct", new { id = product.Id }, Request.Scheme);

            return View(product);
        }

        public IActionResult AddLikedproduct(int? id)
        {
            if (id != null)
            {
                var product = db.Products.SingleOrDefault(x => x.Id == id);
                if (product != null)
                {
                    product.Liked += 1;
                    db.SaveChanges();
                }
            }
            return Json(new { success = true });
        }
        [Authorize]
        public IActionResult AddComment(ProductRate productrate) 
        {
            try
            {
                var CusID = HttpContext.Session.GetString("CustomerId");
                if (CusID != null)
                {
                    var acc = db.Accounts.SingleOrDefault(x => x.Id == int.Parse(CusID))?.UserName;
                    productrate.IdCus = int.Parse(CusID);
                    productrate.CusName = acc;
                    db.ProductRates.Add(productrate);
                    db.SaveChanges();
                    notyfService.Success("Bạn vừa đăng bình luận !");
                    
                    return RedirectToAction("Detail","cusproduct",new {id=productrate.ProductId});
                   
                }
                else
                {
                    notyfService.Error("Có lỗi xảy ra");
                    return RedirectToAction("Detail", "cusproduct", new { id = productrate.ProductId });
                   /* return Json(new { success = false });*/
                }    
                
               
            }
            catch
            {
                notyfService.Error("Có lỗi xãy ra");
                return RedirectToAction("Detail", "cusproduct", new { id = productrate.ProductId });
               /* return Json(new { success = false });*/
            }
           
        }
    }
}
