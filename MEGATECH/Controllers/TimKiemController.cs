using MEGATECH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MEGATECH.Controllers
{
    public class TimKiemController : Controller
    {
        private MEGATECHDBContext db = new MEGATECHDBContext();
        // GET: TimKiem
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(string searchString)
        {
            ViewBag.searchString = searchString;
            var items = db.products.ToList();
            var product = items.Where(p => p.Title.ToLower().Contains(searchString.ToLower()));
            if (product != null)
            {
                return View(product);
            }
            return View();
        }
    }
}