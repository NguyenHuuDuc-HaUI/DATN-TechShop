using MEGATECH.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MEGATECH.Areas.Admin.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Admin/Error
        public ActionResult Index()
        {
            return View();
        }
        [AdminAuthorize()]
        public ActionResult NoFunction() 
        {
            return View();
        }
    }
}