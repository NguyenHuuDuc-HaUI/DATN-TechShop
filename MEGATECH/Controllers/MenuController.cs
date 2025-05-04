using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MEGATECH.Models;
using MEGATECH.Models.EF;

namespace MEGATECH.Controllers
{
    public class MenuController : Controller
    {
        private MEGATECHDBContext db = new MEGATECHDBContext();
        // GET: Menu
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MenuTop()
        {
            var items = db.categories
                .Where(x => x.IsActive)
                .OrderBy(x => x.Position).ToList();
            return PartialView("_MenuTop", items);
        }

        public ActionResult MenuProductCategory()
        {
            var items = db.productCategories.ToList();
            return PartialView("_MenuProductCategory", items);
        }
        public ActionResult MenuLeft(string id)
        {
            if (id != null)
            {
                ViewBag.CateId = id;
            }
            var items = db.productCategories.ToList();
            return PartialView("_MenuLeft", items);
        }

        public ActionResult MenuSupplier(string id)
        {
            if (id != null)
            {
                ViewBag.SupplierId = id;
            }
            var items = db.nhaCungCaps.ToList();
            return PartialView("_MenuSupplier", items);
        }

        public ActionResult MenuArrivals()
        {
            var items = db.productCategories.ToList();
            return PartialView("_MenuArrivals", items);
        }

    }
}