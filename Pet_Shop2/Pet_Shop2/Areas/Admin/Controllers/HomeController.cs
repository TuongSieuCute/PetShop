using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pet_Shop2.Areas.Admin.Models;
using Pet_Shop2.Extensions;
using Pet_Shop2.Models;
using System.Globalization;
using System.Linq;

namespace Pet_Shop2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [LoginRequired]
    public class HomeController : Controller
    {
        PetShopContext db;
        public HomeController(PetShopContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            DateTime today = DateTime.Now.Date;
            DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            DateTime endOfWeek = startOfWeek.AddDays(6);

            DateTime startOfLastWeek = today.AddDays(-(int)today.DayOfWeek - 7 + (int)DayOfWeek.Monday);
            DateTime endOfLastWeek = startOfLastWeek.AddDays(6);
            //Tổng số lượng tuần này
            ViewBag.TotalNumber = db.Orders.Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Date >= startOfWeek && o.OrderDate.Value.Date <= endOfWeek).Count();
            
            //Tổng tiền tuần trước
            var totalMoney = db.OrderDetails
                .Join(
                        db.Orders,
                        od => od.OrderId,
                        o => o.Id,
                        (od, o) => new
                        {
                            OrderDate = o.OrderDate,
                            Total = od.Total
                        })
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Date >= startOfWeek && o.OrderDate.Value.Date <= endOfWeek)
                .Sum(o => o.Total);
            ViewBag.totalMoney = totalMoney;
            //Tổng tiền tuần trước
            var LasttotalMoney = db.OrderDetails
                .Join(
                        db.Orders,
                        od => od.OrderId,
                        o => o.Id,
                        (od, o) => new
                        {
                            OrderDate = o.OrderDate,
                            Total = od.Total
                        })
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Date >= startOfLastWeek && o.OrderDate.Value.Date <= endOfLastWeek)
                .Sum(o => o.Total);

            ViewBag.LasttotalMoney = LasttotalMoney;

            var listorder = db.OrderDetails
                .GroupBy(od => od.ProductId)
                .Select(group => new
                {
                    ProductID = group.Key,
                    TotalQuantity = group.Sum(od => od.Quantity)
                })
                .ToList(); 
            List<ProductViewModel> lstproduct = new List<ProductViewModel>();
            foreach( var item in listorder)
            {
                ProductViewModel prdview = new ProductViewModel();
                prdview.product = db.Products.SingleOrDefault(x=>x.Id== item.ProductID);
                prdview.quantity = (int)item.TotalQuantity;
                prdview.TotalMoney = (int)prdview.product.Price * (int)item.TotalQuantity;
                lstproduct.Add(prdview);
            }
            lstproduct = lstproduct.OrderByDescending(x => x.TotalMoney).Take(5).ToList();
            ViewBag.LstProduct = lstproduct;
            

            return View();


        }
        // Lấy dữ liệu lên Chart.js    
        public List<object> GetData()
        {
            //Lấy doanh thu của các sản phẩm để tính lợi nhuận từng ngày / tháng / năm => bar 
            List<object> data = new List<object>();
            List<string> labels = new List<string>()
            {
                "CN","Thứ 2","Thứ 3","Thứ 4","Thứ 5","Thứ 6","Thứ 7"
            };
            //Lấy dữ liệu danh sách số lượng mua hàng tuần này
            DateTime today = DateTime.Now.Date;
            DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            DateTime endOfWeek = startOfWeek.AddDays(6);

            DateTime startOfLastWeek = today.AddDays(-(int)today.DayOfWeek - 7 + (int)DayOfWeek.Monday);
            DateTime endOfLastWeek = startOfLastWeek.AddDays(6);
            var orders = db.Orders
                .AsEnumerable()
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Date >= startOfWeek && o.OrderDate.Value.Date <= endOfWeek)
                .ToList();

            var NumberCart = new List<int>();

            foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
            {
                var orderCount = orders.Count(o => o.OrderDate.Value.DayOfWeek == dayOfWeek);
                NumberCart.Add(orderCount);
            }
            //Lấy dữ liệu danh sách số lượng mua hàng tuần trước

           

            var Lastorders = db.Orders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Date >= startOfLastWeek && o.OrderDate.Value.Date <= endOfLastWeek)
                .ToList();

            var LastNumberCart = new List<int>();

            foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
            {
                var orderCount = Lastorders.Count(o => o.OrderDate.Value.DayOfWeek == dayOfWeek);
                LastNumberCart.Add(orderCount);
            }


            double percent = (NumberCart.Sum()*1.0 / LastNumberCart.Sum())-1;
            percent = Math.Round(percent, 1);
            //Thêm nhãn từ T2 -> CN
            data.Add(labels);
            //Thêm dữ liệu số lượng mua hàng tuần này
            data.Add(NumberCart);
            //Thêm dữ liệu số lượng tuần trước
            data.Add(LastNumberCart);
            data.Add(percent);
            return data;
        }
        public List<object> getDataPie()
        {
            //Lấy số lượng các sản phẩm đã bán được 
            List<object> data = new List<object>();
            DateTime today = DateTime.Now.Date;
            DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            DateTime endOfWeek = startOfWeek.AddDays(6);

           
            var orderdetail = db.OrderDetails

                .Join(
                db.Orders,
                od => od.OrderId,
                o => o.Id,
                (od, o) => new
                {
                    OrderDate = o.OrderDate,
                    ProductId = od.ProductId,
                    Quantity = od.Quantity
                })
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Date >= startOfWeek && o.OrderDate.Value.Date <= endOfWeek)
                .GroupBy(x => x.ProductId)
                .Select(group => new
                {
                    ProductID = group.Key,
                    TotalQuantity = group.Sum(od => od.Quantity)
                })
                .ToList();

            var datadetail = db.Products
               .AsEnumerable()
               .Join(
                orderdetail,
                product => product.Id,
                grouped => grouped.ProductID,
                (product, grouped) => new
                {
                    ProductName = product.ProductName,
                    ProductID = grouped.ProductID,
                    TotalQuantity = grouped.TotalQuantity
                }
               ).ToList();

            
            

            var productNames = datadetail.Select(item => item.ProductName).ToList();
            var productIDs = datadetail.Select(item => item.ProductID).ToList();
            var quantities = datadetail.Select(item => item.TotalQuantity).ToList();

            

            data.Add(productNames);
            data.Add(productIDs);
            data.Add(quantities);


            return data;
        }

    }
}
