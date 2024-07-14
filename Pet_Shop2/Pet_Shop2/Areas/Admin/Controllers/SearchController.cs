using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Pet_Shop2.Helper;
using Pet_Shop2.Models;


namespace Pet_Shop2.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SearchController : Controller
    {
        private readonly PetShopContext db;
        private readonly IServiceProvider serviceProvider;
        public SearchController(PetShopContext db, IServiceProvider serviceProvider)
        {
            this.db = db;
            this.serviceProvider = serviceProvider;
        }
        [HttpPost]
        public IActionResult FindProduct(string keyword = "", string active = "all", int IDcategory = -1, bool orderby = true)
        {
            List<Product> products = new List<Product>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                products = db.Products.Include(x => x.Category).OrderBy(x => x.Price).ToList();
                if (IDcategory != -1 && IDcategory != 0) products = products.Where(x => x.CategoryId == IDcategory).ToList();
                if (active == "false") products = products.Where(x => x.Active == false).ToList();
                if (orderby == false) products = products.OrderByDescending(x => x.Price).ToList();
                return PartialView("listproductssearchpartial", products);
            }
            products = db.Products
                        .AsNoTracking()
                        .OrderBy(x => x.Price)
                        .Include(x => x.Category)
                        .Where(x => x.ProductName != null && x.ProductName.Contains(keyword))
                        .ToList();
            if (IDcategory != -1 && IDcategory != 0) products = products.Where(x => x.CategoryId == IDcategory).ToList();
            if (active == "false") products = products.Where(x => x.Active == false).ToList();
            if (orderby == false) products = products.OrderByDescending(x => x.Price).ToList();

            if (products != null)
            {
                return PartialView("listproductssearchpartial", products);
            }
            else
            {
                return PartialView("listproductssearchpartial", null);
            }

        }

        [HttpPost]
        public IActionResult FindCustomer(string keyword)
        {
            List<Account> lstAcc = new List<Account>();
            if (string.IsNullOrEmpty(keyword) || keyword.Count() < 1)
            {
                lstAcc = db.Accounts.ToList();

            }
            else
            {
                lstAcc = db.Accounts.Where(x => x.Email != null && x.Email.Contains(keyword) || x.Phone != null && x.Phone.Contains(keyword) || x.FullName != null && x.FullName.Contains(keyword)).ToList();
            }
            if (lstAcc.Count() > 0 && lstAcc != null)
            {
                return PartialView("listcustomersearchpartial", lstAcc);

            }
            else
            {
                return PartialView("listcustomersearchpartial", null);
            }

        }
        [HttpPost]
        public IActionResult FindOrder(string keyword, int phuongthuc = -1, int trangthai = -1, bool order = true)
        {

            List<Order> lstorder = new List<Order>();
            lstorder = db.Orders.Include(x => x.TransctStatus).ToList();
            if (string.IsNullOrEmpty(keyword) || keyword.Count() < 1)
            {

            }
            else
            {
                lstorder = db.Orders.Where(x => x.Address != null && x.Address.Contains(keyword) || x.Id.ToString() == keyword).ToList();
            }
            if (trangthai != -1)
            {
                //Hủy đơn 
                lstorder = lstorder.Where(x => x.TransctStatusId == trangthai).ToList();
            }

            if (phuongthuc == 1)
            {
                lstorder = lstorder.Where(x => x.Paid == true).ToList();
            }
            else if (phuongthuc == 0)
            {
                lstorder = lstorder.Where(x => x.Paid == false).ToList();
            }

            if (!order)
            {
                lstorder = lstorder.OrderByDescending(x => x.Id).ToList();
            }
            if (lstorder.Count() > 0 && lstorder != null)
            {
                return PartialView("listordersearchpartial", lstorder);

            }
            else
            {
                return PartialView("listordersearchpartial", null);
            }
        }
        [HttpPost]
        public IActionResult FindComment(string keyword, bool orderby = true)
        {
            List<ProductRate> lstComment = db.ProductRates.ToList();
            if (keyword != null && keyword.Length > 0)
            {
                lstComment = lstComment.Where(x=>x.Id.ToString()==keyword || x.ProductId.ToString()==keyword || x.Comment==keyword).ToList();

            }
            if(!orderby)
            {
                lstComment = lstComment.OrderByDescending(x => x.Rating).ToList();
            }
            else
            {
                lstComment = lstComment.OrderBy(x => x.Rating).ToList();
            }    
            return PartialView("listcommentsearchpartial", lstComment);
        }

    }
}
