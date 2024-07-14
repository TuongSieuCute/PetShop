using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pet_Shop2.Models;
using Pet_Shop2.ModelsView;
using System;
using System.Security.Cryptography;

namespace Pet_Shop2.Controllers
{
    [Authorize]
    public class Checkout : Controller
    {
        PetShopContext db;
        INotyfService notyfService;
        public Checkout(PetShopContext db, INotyfService notyfService)
        {
            this.db = db;
            this.notyfService = notyfService;
        }
        public IActionResult Index()
        {
            ViewBag.tinh = db.Locations.ToList();
            ViewBag.lstGioHang = HttpContext.Session.Get<List<CartItem>>("GioHang");
            ViewBag.CusID = HttpContext.Session.GetString("CustomerId");
            if (ViewBag.CusID != null)
                ViewBag.Acc = HttpContext.Session.GetString("UserName");


            var CusID = HttpContext.Session.GetString("CustomerId");
            if (CusID != null)
                ViewBag.Acc = db.Accounts.SingleOrDefault(x => x.Id == int.Parse(CusID));
            if ( CusID !=null)
            {
                var customer = db.Accounts.SingleOrDefault(x => x.Id == int.Parse(CusID));
                return View(customer);
            }    
            return View(null);  
            
        }
        [HttpPost]
        public IActionResult Themdonhang(string ordernote = "", int _province = 0, int _district = 0, int _ward = 0, string stresshouse = "")
        {

            var lstCart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var IDCus = HttpContext.Session.GetString("CustomerId");
            
           
            string? province = db.Locations.SingleOrDefault(x => x.Id == _province)?.Name;
            string? district = db.Districts.SingleOrDefault(x => x.Id == _district)?.Name;
            string? ward = db.Wards.SingleOrDefault(x => x.WardId == _ward)?.Name;
            var orderid = -1;
            if (IDCus != null)
            {
                var Acc = db.Accounts.SingleOrDefault(x => x.Id == int.Parse(IDCus));
                if (Acc != null)
                {
                    Acc.Location = province;
                    Acc.District = district;
                    Acc.Ward = ward;
                    Acc.Address = province + "," + district + "," + ward + "," + stresshouse;
                    db.SaveChanges();
                }
                Order ord = new Order()
                {

                    AccountId = Acc?.Id,
                    OrderDate = DateTime.Now,
                    ShipDate = DateTime.Now.AddDays(3),
                    Deleted = false,
                    Paid = false,
                    Note = ordernote,
                    TransctStatusId=1,
                    Address = province + "," + district + "," + ward + "," + stresshouse
                };
                db.Orders.Add(ord);
               
                db.SaveChanges();
                orderid = ord.Id;
                foreach (var item in lstCart)
                {
                    OrderDetail orderDetail = new OrderDetail()
                    {
                        OrderId = ord.Id,
                        ProductId = item?.product?.Id,
                        Quantity = item?.amount,
                        Total = (decimal)item.TotalMoney

                    };
                    db.OrderDetails.Add(orderDetail);
                    db.SaveChanges();
                }
            }
            

            
            
            lstCart = null;
            HttpContext.Session.Set<List<CartItem>>("GioHang", lstCart);
            return Json(new {success = true, OrderId= orderid });
        }
    }
}
