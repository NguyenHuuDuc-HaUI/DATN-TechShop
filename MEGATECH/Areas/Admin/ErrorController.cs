using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MEGATECH.Areas.Admin
{
    public class ErrorController : Controller
    {
        // GET: Admin/Error
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult NoFunction()
        {
            return View();
        }
    }
}