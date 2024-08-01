using AspNetCoreHero.ToastNotification.Abstractions;
using Azure;
using ECommerceMVC.Helpers;
using ECommerceMVC.Services;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            var fullAddress = $"{province},{district},{ward},{stresshouse}";

            // Store address information in session
            HttpContext.Session.SetString("Province", province);
            HttpContext.Session.SetString("District", district);
            HttpContext.Session.SetString("Ward", ward);
            HttpContext.Session.SetString("Address", stresshouse);

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
                    Acc.Address = fullAddress;
                    db.SaveChanges();
                }
            }

            Order ord = new Order()
            {
                AccountId = int.Parse(IDCus),
                OrderDate = DateTime.Now,
                ShipDate = DateTime.Now.AddDays(3),
                Deleted = false,
                Paid = false,
                Note = ordernote,
                TransctStatusId = 1,
                Address = fullAddress
            };

            db.Orders.Add(ord);
            db.SaveChanges();

            var orderId = ord.Id;
            foreach (var item in lstCart)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderId = orderId,
                    ProductId = item?.product?.Id,
                    Quantity = item?.amount,
                    Total = (decimal)item.TotalMoney
                };
                db.OrderDetails.Add(orderDetail);
            }
            db.SaveChanges();

            // Clear the cart from session
            HttpContext.Session.Set<List<CartItem>>("GioHang", null);

            return Json(new { success = true, OrderId =ord.Id });
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

            // Set address information in session
            var province = HttpContext.Session.GetString("Province");
            var district = HttpContext.Session.GetString("District");
            var ward = HttpContext.Session.GetString("Ward");
            var address = HttpContext.Session.GetString("Address");

            if (string.IsNullOrEmpty(province) || string.IsNullOrEmpty(district) || string.IsNullOrEmpty(ward) || string.IsNullOrEmpty(address))
            {
                var customer = db.Accounts.SingleOrDefault(x => x.Id == int.Parse(HttpContext.Session.GetString("CustomerId")));

            }

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
                            // Get address information from session
                            var province = HttpContext.Session.GetString("Province");
                            var district = HttpContext.Session.GetString("District");
                            var ward = HttpContext.Session.GetString("Ward");
                            var address = HttpContext.Session.GetString("Address");
                            var fullAddress = $"{province},{district},{ward},{address}";

                            Acc.Location = province;
                            Acc.District = district;
                            Acc.Ward = ward;
                            Acc.Address = fullAddress;
                            db.SaveChanges();

                            Order ord = new Order()
                            {
                                AccountId = Acc.Id,
                                OrderDate = DateTime.Now,
                                ShipDate = DateTime.Now.AddDays(3),
                                Deleted = false,
                                Paid = true,
                                PaymentDate = DateTime.Now,
                                PaymentId = 0,
                                Note = "Paid via PayPal",
                                TransctStatusId = 2,
                                Address = fullAddress
                            };
                            db.Orders.Add(ord);
                            db.SaveChanges();
                            ord.PaymentId = ord.Id;
                            db.Orders.Update(ord);
                            

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

                            // Clear the cart from session
                            HttpContext.Session.Set<List<CartItem>>("GioHang", null);
                        }
                    }

                    return Ok(response);
                }

                return BadRequest(response);
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