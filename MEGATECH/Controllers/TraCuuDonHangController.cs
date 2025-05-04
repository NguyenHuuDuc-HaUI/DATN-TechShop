using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MEGATECH.Models;
using MEGATECH.Models.EF; // Import model classes

namespace MEGATECH.Controllers
{
    public class TraCuuDonHangController : Controller
    {
        private MEGATECHDBContext db = new MEGATECHDBContext(); // Initialize database context

        // GET: TraCuuDonHang
        public ActionResult Index()
        {
            return View();
        }
        // Action để xử lý tìm kiếm hoá đơn và trả về một Partial View
        [HttpPost]
        // Phương thức tìm kiếm theo mã hoá đơn
        public ActionResult TimBangMa(string searchType, string maHoaDon)
        {
            var hoaDons = db.hoaDons.Where(h => h.MaHoaDon == maHoaDon).ToList();
            return View("Index", hoaDons);
        }
        [HttpPost]
        // Phương thức tìm kiếm theo tên khách hàng và số CCCD
        public ActionResult TimBangTen(string searchType, string tenKhachHang, string CCCD)
        {
            var hoaDons = db.hoaDons.Where(h => h.TenKhachHang == tenKhachHang && h.CCCD == CCCD).ToList();
            return View("Index", hoaDons);
        }

    }
}
