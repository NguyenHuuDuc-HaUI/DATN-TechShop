using MEGATECH.Models;
using MEGATECH.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MEGATECH.App_Start
{
    public class AdminAuthorize : AuthorizeAttribute
    {

        MEGATECHDBContext db = new MEGATECHDBContext();
        public string idChucNang { get; set; }
        public string chucVu { get; set; }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var user = (NhanVien)HttpContext.Current.Session["user"];
            // Nếu user chưa có trong session, thì kiểm tra Cookie
            if (user == null)
            {
                string username = CookieHelper.Get("username-megatech");
                string password = CookieHelper.Get("password-megatech");
                var taiKhoan = db.nhanViens.SingleOrDefault(x => x.TenDangNhap.ToLower() == username.ToLower());
                if (taiKhoan != null && taiKhoan.MatKhau == password)
                {
                    user = taiKhoan;
                    HttpContext.Current.Session["user"] = taiKhoan; // Đặt lại session
                }
            }

            // Nếu vẫn chưa có session, thì phải đăng nhập
            if (user == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new
                        {
                            controller = "Home",
                            action = "Login",
                            area = "Admin"
                        })
                    );
                return;
            }

            //Nếu không diều chức năng => Cho phép chạy
            if (String.IsNullOrEmpty(idChucNang) && String.IsNullOrEmpty(chucVu))
            {
                return;
            }

            // Kiểm tra quyền chức năng
            if (!String.IsNullOrEmpty(idChucNang))
            {
                var phanQuyen = db.phanQuyens.Count(x => x.IDChucVu == user.ID_ChucVu && x.MaChucNang == idChucNang);
                if (phanQuyen <= 0)
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new System.Web.Routing.RouteValueDictionary(
                            new
                            {
                                controller = "Error",
                                action = "NoFunction",
                                area = "Admin"
                            })
                    );
                    return;
                }
            }

            // Kiểm tra quyền chức vụ nếu có
            if (!String.IsNullOrEmpty(chucVu))
            {
                var lsQuyen = chucVu.Split(',').Select(r => r.Trim()).ToList();
                if (user.ChucVu == null || !lsQuyen.Contains(user.ChucVu.TenChucVu, StringComparer.InvariantCultureIgnoreCase))
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new System.Web.Routing.RouteValueDictionary(
                            new
                            {
                                controller = "Error",
                                action = "NoFunction",
                                area = "Admin"
                            })
                    );
                    return;
                }
            }
        }
    }
}