using AspNetCoreHero.ToastNotification.Abstractions;
using ECommerceMVC.Helpers;
using ECommerceMVC.Services;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pet_Shop2.Models;
using Pet_Shop2.ModelsView;
using System;
using System.Globalization;
using System.Security.Cryptography;

namespace Pet_Shop2.Controllers
{
    [Authorize]
    public class Checkout : Controller
    {
        private readonly PaypalClient paypalClient;
        PetShopContext db;
        INotyfService notyfService;
        private readonly IVnPayService _vnPayservice;

        public Checkout(PetShopContext db, INotyfService notyfService, PaypalClient paypalClient, IVnPayService vnPayservice)
        {
            this.paypalClient = paypalClient;
            this.db = db;
            this.notyfService = notyfService;
            _vnPayservice = vnPayservice;
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
            if (CusID != null)
            {
                var customer = db.Accounts.SingleOrDefault(x => x.Id == int.Parse(CusID));
                ViewBag.PayPalClientId = TempData["PayPalClientId"];
                return View(customer);
            }
            return View(null);

        }
        [HttpPost]
        public IActionResult Themdonhang(string ordernote = "", int _province = 0, int _district = 0, int _ward = 0, string stresshouse = "", string payment = "COD")
        {

            var lstCart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var IDCus = HttpContext.Session.GetString("CustomerId");


            string? province = db.Locations.SingleOrDefault(x => x.Id == _province)?.Name;
            string? district = db.Districts.SingleOrDefault(x => x.Id == _district)?.Name;
            string? ward = db.Wards.FirstOrDefault(x => x.WardId == _ward)?.Name;
            var orderid = -1;
            if (IDCus != null)
            {
                var Acc = db.Accounts.SingleOrDefault(x => x.Id == int.Parse(IDCus));
                if (Acc != null)
                {
                    if (payment == "Thanh toán VNPay")
                    {
                        var vnPayModel = new VnPaymentRequestModel
                        {
                            Amount = lstCart.Sum(p => p.TotalMoney),
                            CreatedDate = DateTime.Now,
                            Description = $"{Acc.FullName} {Acc.Phone}",
                            FullName = Acc.FullName,
                            OrderId = new Random().Next(1000, 100000)
                        };
                        return Redirect(_vnPayservice.CreatePaymentUrl(HttpContext, vnPayModel));
                    }

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
                    TransctStatusId = 1,
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
            return Json(new { success = true, OrderId = orderid });
        }
        [Authorize]
        [HttpGet]

        public IActionResult PaymentSuccess()
        {
            return View("Success");
        }

        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>("GioHang");

        [HttpPost("/Cart/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
        {
            // Thông tin đơn hàng gửi qua Paypal
            var tongTienUSD = Convert.ToDouble(
                (Cart.Sum(p => p.TotalMoney) / 25315)
                    .ToString("C", new CultureInfo("en-US"))
                    .Remove(0, 1)
            );

            var tongTien = tongTienUSD.ToString();
            var donViTienTe = "USD";
            var maDonHangThamChieu = "DH" + DateTime.Now.Ticks.ToString();

            try
            {
                var response = await paypalClient.CreateOrder(tongTien, donViTienTe, maDonHangThamChieu);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }


        [Authorize]
        [HttpPost("/Cart/capture-paypal-order")]
        public async Task<IActionResult> CapturePaypalOrder(string orderID, CancellationToken cancellationToken)
        {
            try
            {
                var response = await paypalClient.CaptureOrder(orderID);
                if (response.status == "COMPLETED")
                {
                    var lstCart = HttpContext.Session.Get<List<CartItem>>("GioHang");
                    var IDCus = HttpContext.Session.GetString("CustomerId");

                    if (IDCus != null)
                    {
                        var Acc = db.Accounts.SingleOrDefault(x => x.Id == int.Parse(IDCus));
                        if (Acc != null)
                        {
                            Order ord = new Order()
                            {
                                AccountId = Acc.Id,
                                OrderDate = DateTime.Now,
                                ShipDate = DateTime.Now.AddDays(3),
                                Deleted = false,
                                Paid = true,
                                PaymentDate = DateTime.Now,
                                Note = "Paid via PayPal",
                                TransctStatusId = 2,
                                Address = Acc.Address
                            };
                            db.Orders.Add(ord);
                            db.SaveChanges();

                            foreach (var item in lstCart)
                            {
                                OrderDetail orderDetail = new OrderDetail()
                                {
                                    OrderId = ord.Id,
                                    ProductId = item.product.Id,
                                    Quantity = item.amount,
                                    Total = (decimal)item.TotalMoney
                                };
                                db.OrderDetails.Add(orderDetail);
                            }
                            db.SaveChanges();
                            HttpContext.Session.Set("GioHang", null); 
                        }
                    }
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }

        [Authorize]
        public IActionResult PaymentFail() {
            return View();
        }

        [Authorize]
        public IActionResult PaymentCallBack() {
            var response = _vnPayservice.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00") {
                TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }

            // Lưu đơn hàng vô database

            TempData["Message"] = $"Thanh toán VNPay thành công";
            return RedirectToAction("PaymentSuccess");
        }
    }
}
