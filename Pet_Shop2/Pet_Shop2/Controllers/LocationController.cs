using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pet_Shop2.Models;

namespace Pet_Shop2.Controllers
{
    public class LocationController : Controller
    {


        PetShopContext db;
        public LocationController(PetShopContext db)
        {
            this.db = db;
        }
        public IActionResult DistrictList(int LocationID)
        {
            var District = db.Districts.Where(x=>x.LocationId==LocationID).OrderBy(x=>x.Name).ToList();
            return Json(District);
        }
        public IActionResult WardList(int DistrictID)
        {
            var ward = db.Wards.Where(x => x.DistrictId == DistrictID).OrderBy(x => x.Name).ToList();
            return Json(ward);
        }
    }
}
