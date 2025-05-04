using MEGATECH.App_Start;
using MEGATECH.Models;
using MEGATECH.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MEGATECH.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private MEGATECHDBContext db = new MEGATECHDBContext();
        // GET: Admin/Home
        [AdminAuthorize()]

        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Login", "TaiKhoan");
            }
            else
            {
                return View();
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) == true || string.IsNullOrEmpty(password) == true)
            {
                TempData["error"] = "Tên đăng nhập hoặc mật khẩu không được để trống!";
                return View();
            }
            var taiKhoan = db.nhanViens.SingleOrDefault(x => x.TenDangNhap.ToLower() == username.ToLower());
            if (taiKhoan == null)
            {
                TempData["error"] = "Tài khoản này chưa được tạo!";
                return View();
            }
            if (taiKhoan.TenDangNhap != username)
            {
                TempData["error"] = "Tên đăng nhập không đúng!";
                return View();
            }

            if (taiKhoan.MatKhau != password)
            {
                TempData["error"] = "Mật khẩu đăng nhập không đúng!";
                return View();
            }
            if (taiKhoan.IsActiveAccount == false)
            {
                TempData["error"] = "Tải khoản đã bị khoá!";
                return View();
            }
            // Tài khoản đăng nhập: lưu vào session server
            Session["user"] = taiKhoan;

            // Lưu cookie
            CookieHelper.Create("username-megatech", taiKhoan.TenDangNhap, DateTime.Now.AddDays(10));
            CookieHelper.Create("password-megatech", taiKhoan.MatKhau, DateTime.Now.AddDays(10));

            return RedirectToAction("Index", "Home");
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        public ActionResult Logout()
        {
            //Xoá session
            Session.Remove("user");

            //Xoá cookie
            CookieHelper.Remove("username-megatech");
            CookieHelper.Remove("password-megatech");
            //Xoá session form
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}